using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using EMRSystem.Deposit.Dto;
using EMRSystem.Invoices;
using EMRSystem.Patients;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public class DepositAppService : AsyncCrudAppService<Deposit, DepositDto, long, PagedDepositResultRequestDto, CreateUpdateDepositDto, CreateUpdateDepositDto>, IDepositAppService
    {
        private readonly IRepository<Deposit, long> _repository;
        private readonly IRepository<EMRSystem.Admission.Admission, long> _admissionRepo;
        private readonly IRepository<Patient, long> _patientRepo;


        public DepositAppService(IRepository<Deposit, long> repository, IRepository<Patient, long> patientRepo, IRepository<EMRSystem.Admission.Admission, long> admissionRepo) : base(repository)
        {
            _repository = repository;
            _admissionRepo = admissionRepo;
            _patientRepo = patientRepo;

        }

        protected override IQueryable<Deposit> CreateFilteredQuery(PagedDepositResultRequestDto input)
        {
            return _repository.GetAll()
                .Include(x => x.Patient)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => EF.Functions.Like(x.Patient.FullName, $"%{input.Keyword}%"));
        }


        //public override async Task<DepositDto> CreateAsync(CreateUpdateDepositDto input)
        //{
        //    // basic validation: admission must exist
        //    var patient = await _patientRepo.FirstOrDefaultAsync(input.PatientId);
        //    if (patient == null)
        //        throw new UserFriendlyException("Patient not found");

        //    var deposit = ObjectMapper.Map<Deposit>(input);
        //    await _repository.InsertAsync(deposit);
        //    await CurrentUnitOfWork.SaveChangesAsync();
        //    return MapToEntityDto(deposit);
        //}

        public async Task<DepositResponseDto> CreateDepositWithStripeSupportAsync(CreateUpdateDepositDto input)
        {
            var patient = await _patientRepo.FirstOrDefaultAsync(input.PatientId);
            if (patient == null)
                throw new UserFriendlyException("Patient not found");

            if (input.PaymentMethod == PaymentMethod.Card)
            {
                if (input.Amount <= 0)
                    throw new UserFriendlyException("Deposit amount must be greater than 0");

                string baseUrl = "http://localhost:4200/app/deposit";
                string successUrl = $"{baseUrl}?payment=success&patientId={input.PatientId}";
                string cancelUrl = $"{baseUrl}?payment=cancel&patientId={input.PatientId}";

                string stripeUrl = await CreateStripeCheckoutSessionForDeposit(
                                    input,
                                    successUrl,
                                    cancelUrl
                                );


                return new DepositResponseDto
                {
                    Deposit = null,
                    StripeRedirectUrl = stripeUrl
                };
            }

            // Normal deposit
            var deposit = ObjectMapper.Map<Deposit>(input);
            await _repository.InsertAsync(deposit);
            await CurrentUnitOfWork.SaveChangesAsync();

            return new DepositResponseDto
            {
                Deposit = MapToEntityDto(deposit),
                StripeRedirectUrl = null
            };
        }

        public async Task<string> CreateStripeCheckoutSessionForDeposit(CreateUpdateDepositDto input, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
    {
        new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(input.Amount * 100),
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = $"Patient Deposit #{input.PatientId}",
                    Description = "Deposit for Admit Patient"
                },
            },
            Quantity = 1,
        }
    },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
    {
        { "patientId", input.PatientId.ToString() },
        { "amount", input.Amount.ToString() },
        { "tenantId", AbpSession.TenantId?.ToString() ?? "1" },
        { "billingMethod", input.BillingMethod.ToString() },
        { "depositDateTime", input.DepositDateTime.ToString("o") }, // ISO 8601 format
        { "purpose", "deposit" }
    }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;

        }

    }

}
