using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Deposit.Dto;
using EMRSystem.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public class DepositTransactionAppService :
        AsyncCrudAppService<DepositTransaction, DepositTransactionDto, long, PagedAndSortedResultRequestDto, CreateUpdateDepositTransactionDto>,
        IDepositTransactionAppService
    {
        private readonly IRepository<PatientDeposit, long> _patientDepositRepository;
        private readonly IConfiguration _configuration;
        public DepositTransactionAppService(IRepository<DepositTransaction, long> repository,
             IRepository<PatientDeposit, long> patientDepositRepository,
             IConfiguration configuration) : base(repository)
        {
            _patientDepositRepository = patientDepositRepository;
            _configuration = configuration;
        }

        public async Task<string> CreateDepositTransactionAsync(CreateUpdateDepositTransactionDto input)
        {
            if (input.PaymentMethod == PaymentMethod.Cash)
            {
                // 🔹 Direct save
                var transaction = ObjectMapper.Map<DepositTransaction>(input);
                transaction.IsPaid = true;
                transaction.Description = "Credit Deposit.";
                transaction.TransactionDate = DateTime.Now;
                transaction.TransactionType = TransactionType.Credit;
                transaction.ReceiptNo = await GenerateReceiptNoAsync(input.TenantId);


                await Repository.InsertAsync(transaction);

                // 🔹 Update PatientDeposit summary
                var patientDeposit = await _patientDepositRepository.GetAsync(input.PatientDepositId);
                patientDeposit.TotalCreditAmount += input.Amount;
                patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

                await _patientDepositRepository.UpdateAsync(patientDeposit);

                return null; // cash case me checkout url ki zarurat nahi
            }
            else if (input.PaymentMethod == PaymentMethod.Card)
            {
                // 🔹 Stripe Checkout session create
                var domain = _configuration["App:ClientRootAddress"]; // frontend base url

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
                            UnitAmount = (long)(input.Amount * 100), // cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Patient Deposit"
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

                // 🔹 frontend ko redirect url return karo
                return session.Url;
            }

            throw new UserFriendlyException("Invalid Payment Method");
        }
        public async Task<string> GenerateReceiptNoAsync(int tenantId)
        {
            // Tenant ke saare transactions dekh kar last ReceiptNo nikalo
            var lastTransaction = await Repository.GetAll()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (lastTransaction == null || string.IsNullOrEmpty(lastTransaction.ReceiptNo))
            {
                return $"RCPT-{tenantId}-00001";
            }

            // Last receipt extract karke increment karo
            var lastNoPart = lastTransaction.ReceiptNo.Split('-').Last();
            int lastNumber;
            if (!int.TryParse(lastNoPart, out lastNumber))
            {
                lastNumber = 0;
            }

            return $"RCPT-{tenantId}-{(lastNumber + 1).ToString("D5")}";
        }

        public async Task<ListResultDto<DepositTransactionDto>> GetAllByPatientDepositAsync(long patientDepositId)
        {
            // pehle confirm karo PatientDeposit exist karta hai ya nahi
            var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(patientDepositId);
            if (patientDeposit == null)
            {
                throw new UserFriendlyException("Patient deposit not found");
            }

            // sirf wahi transactions lao jisme IsPaid true hai
            var transactions = await Repository
                .GetAll()
                .Where(t => t.PatientDepositId == patientDepositId && t.IsPaid)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            // map karo DTO me
            var transactionDtos = ObjectMapper.Map<List<DepositTransactionDto>>(transactions);

            return new ListResultDto<DepositTransactionDto>(transactionDtos);
        }


    }
}
