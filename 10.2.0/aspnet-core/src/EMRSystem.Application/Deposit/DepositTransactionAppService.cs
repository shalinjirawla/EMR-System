using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Deposit.Dto;
using EMRSystem.Invoices;
using EMRSystem.NumberingService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentMethod = EMRSystem.Invoices.PaymentMethod;

namespace EMRSystem.Deposit
{
    public class DepositTransactionAppService :
        AsyncCrudAppService<DepositTransaction, DepositTransactionDto, long, PagedAndSortedResultRequestDto, CreateUpdateDepositTransactionDto>,
        IDepositTransactionAppService
    {
        private readonly IRepository<PatientDeposit, long> _patientDepositRepository;
        private readonly IConfiguration _configuration;
        private readonly INumberingService _numberingService;

        public DepositTransactionAppService(IRepository<DepositTransaction, long> repository,
             IRepository<PatientDeposit, long> patientDepositRepository,
                INumberingService numberingService,
             IConfiguration configuration) : base(repository)
        {
            _patientDepositRepository = patientDepositRepository;
            _numberingService = numberingService;
            _configuration = configuration;
        }

        public async Task<string> CreateDepositTransactionAsync(CreateUpdateDepositTransactionDto input)
        {
            if (input.PaymentMethod == PaymentMethod.Cash)
            {
                var transaction = ObjectMapper.Map<DepositTransaction>(input);
                transaction.IsPaid = true;
                transaction.Description = "Credit Deposit.";
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionType = TransactionType.Credit;
                transaction.ReceiptNo = await GenerateReceiptNoAsync(input.TenantId);
                transaction.PaymentIntentId = null;
                transaction.Amount = input.Amount;

                // ✅ Refund defaults
                transaction.RemainingAmount = input.Amount;
                transaction.RefundedAmount = 0;
                transaction.IsRefund = false;
                transaction.RefundTransactionId = null;
                transaction.RefundDate = null;
                transaction.RefundReceiptNo = null;

                var patientDeposit = await _patientDepositRepository.GetAsync(input.PatientDepositId);

                // ✅ If patient has negative balance, adjust it
                if (patientDeposit.TotalBalance < 0)
                {
                    var negativeBalance = Math.Abs(patientDeposit.TotalBalance);

                    // Kitna adjust ho sakta hai new credit me se
                    var adjustment = Math.Min(transaction.RemainingAmount, negativeBalance);

                    transaction.RemainingAmount -= adjustment;
                }

                // ✅ Add credit normally
                patientDeposit.TotalCreditAmount += input.Amount;
                patientDeposit.TotalBalance =
                    patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

                await Repository.InsertAsync(transaction);
                await _patientDepositRepository.UpdateAsync(patientDeposit);

                return null;
            }

            else if (input.PaymentMethod == PaymentMethod.Card)
            {
                var domain = _configuration["App:ClientRootAddress"];

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "inr",
                            UnitAmount = (long)(input.Amount * 100), 
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Credit Deposit"
                            },
                        },
                        Quantity = 1,
                    },
                },
                    Mode = "payment",
                    SuccessUrl = "http://localhost:4200/app/deposit",
                    CancelUrl = "http://localhost:4200/app/deposit",
                    Metadata = new Dictionary<string, string>
                {
                    { "purpose", "deposit" },
                    { "tenantId", input.TenantId.ToString() },
                    { "patientDepositId", input.PatientDepositId.ToString() },
                    { "amount", input.Amount.ToString() }
                }
                };

                var service = new SessionService();
                var session = service.Create(options);

                
                return session.Url;
            }

            throw new UserFriendlyException("Invalid Payment Method");
        }
        public async Task<string> GenerateReceiptNoAsync(int tenantId)
        {
            return await _numberingService.GenerateReceiptNumberAsync(
                Repository,        
                "DEP-REC",         
                tenantId,
                "ReceiptNo"       
            );
        }

        private async Task<string> GenerateRefundReceiptNoAsync(int tenantId)
        {
            return await _numberingService.GenerateReceiptNumberAsync(
                Repository,
                "DEP-REF",
                tenantId,
                "RefundReceiptNo"
            );
        }


        public async Task<DepositTransactionDto> CreateDepositRefundAsync(long depositTransactionId)
        {
            var originalTxn = await Repository.FirstOrDefaultAsync(x => x.Id == depositTransactionId);
            if (originalTxn == null)
                throw new UserFriendlyException("Deposit transaction not found.");

            if (originalTxn.TransactionType != TransactionType.Credit)
                throw new UserFriendlyException("Only credit transactions can be refunded.");

            if (originalTxn.IsRefund)
                throw new UserFriendlyException("This deposit is already refunded.");

            decimal refundAmount = originalTxn.RemainingAmount;
            if (refundAmount <= 0)
                throw new UserFriendlyException("No refundable amount available.");

            // 🔹 CARD REFUND LOGIC
            string? stripeRefundId = null;
            if (originalTxn.PaymentMethod == PaymentMethod.Card)
            {
                if (string.IsNullOrEmpty(originalTxn.PaymentIntentId))
                    throw new UserFriendlyException("Stripe PaymentIntentId missing — cannot refund.");

                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = originalTxn.PaymentIntentId,
                    Amount = (long)(refundAmount * 100) // paise
                };

                var refundService = new RefundService();
                var refund = await refundService.CreateAsync(refundOptions);

                stripeRefundId = refund.Id; // ✅ Store refund id
            }

            // 🔹 Mark original txn refunded
            originalTxn.IsRefund = true;
            originalTxn.RefundedAmount = refundAmount;
            originalTxn.RemainingAmount = 0;
            originalTxn.RefundDate = DateTime.Now;
            originalTxn.RefundReceiptNo = await GenerateRefundReceiptNoAsync(originalTxn.TenantId);
            originalTxn.RefundTransactionId = stripeRefundId; // null for cash refund

            await Repository.UpdateAsync(originalTxn);

            // 🔹 Create new debit entry (no receipt)
            var refundTxn = new DepositTransaction
            {
                TenantId = originalTxn.TenantId,
                PatientDepositId = originalTxn.PatientDepositId,
                Amount = refundAmount,
                TransactionType = TransactionType.Debit,
                PaymentMethod = originalTxn.PaymentMethod,
                TransactionDate = DateTime.Now,
                Description = $"Refund deposit amount {refundAmount}",
                IsPaid = false,
                RemainingAmount = 0,
                RefundedAmount = 0,
                IsRefund = false,
                RefundTransactionId = null,
                PaymentIntentId = null
            };

            await Repository.InsertAsync(refundTxn);

            // 🔹 Update wallet
            var patientDeposit = await _patientDepositRepository.GetAsync(originalTxn.PatientDepositId);
            patientDeposit.TotalDebitAmount += refundAmount;
            patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

            await _patientDepositRepository.UpdateAsync(patientDeposit);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<DepositTransactionDto>(refundTxn);
        }



        public async Task<ListResultDto<DepositTransactionDto>> GetAllByPatientDepositAsync(long patientDepositId)
        {
          
            var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(patientDepositId);
            if (patientDeposit == null)
            {
                throw new UserFriendlyException("Patient deposit not found");
            }

            var transactions = await Repository
                .GetAll()
                .Where(t => t.PatientDepositId == patientDepositId && t.IsPaid)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var transactionDtos = ObjectMapper.Map<List<DepositTransactionDto>>(transactions);

            return new ListResultDto<DepositTransactionDto>(transactionDtos);
        }
        public async Task<ListResultDto<DepositTransactionDto>> GetAllByPatientDepositTransactionAsync(long patientDepositId)
        {

            var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(patientDepositId);
            if (patientDeposit == null)
            {
                throw new UserFriendlyException("Patient deposit not found");
            }

            var transactions = await Repository
                .GetAll()
                .Where(t => t.PatientDepositId == patientDepositId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            var transactionDtos = ObjectMapper.Map<List<DepositTransactionDto>>(transactions);

            return new ListResultDto<DepositTransactionDto>(transactionDtos);
        }


    }
}
