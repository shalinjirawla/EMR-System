using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Deposit.Dto;
using EMRSystem.Invoices;
using EMRSystem.NumberingService;
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


                await Repository.InsertAsync(transaction);

                var patientDeposit = await _patientDepositRepository.GetAsync(input.PatientDepositId);
                patientDeposit.TotalCreditAmount += input.Amount;
                patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

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
