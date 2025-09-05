using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AutoMapper.Internal.Mappers;
using EMRSystem.EmergencyProcedure;
using EMRSystem.Invoices;
using EMRSystem.NumberingService;
using EMRSystem.ProcedureReceipts.Dto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts
{
    public class ProcedureReceiptAppService :
        AsyncCrudAppService<ProcedureReceipt, ProcedureReceiptDto, long, PagedAndSortedResultRequestDto, CreateUpdateProcedureReceiptDto, CreateUpdateProcedureReceiptDto>,
        IProcedureReceiptAppService
    {
        private readonly IRepository<SelectedEmergencyProcedures, long> _selectedProcedureRepository;
        private readonly INumberingService _numberingService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public ProcedureReceiptAppService(
            IRepository<ProcedureReceipt, long> repository,
            IRepository<SelectedEmergencyProcedures, long> selectedProcedureRepository,
            INumberingService numberingService,

        IUnitOfWorkManager unitOfWorkManager)
            : base(repository)
        {
            _selectedProcedureRepository = selectedProcedureRepository;
            _numberingService = numberingService;
            _unitOfWorkManager = unitOfWorkManager;
        }
        protected override IQueryable<ProcedureReceipt> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Patient); // 👈 Patient include kiya
        }


        public async Task<ProcedureReceiptDto> CreateProcedureReceiptAsync(CreateProcedureReceiptWithIdsDto dto)
        {
            var input = dto.Input;
            var selectedProcedureIds = dto.SelectedProcedureIds;
            var entity = ObjectMapper.Map<ProcedureReceipt>(input);
            entity.ReceiptNumber = await GenerateProcedureReceiptNoAsync(input.TenantId);
            entity.Status = InvoiceStatus.Paid;
            entity.PaymentIntentId = null;

            await Repository.InsertAndGetIdAsync(entity);

            // update selected emergency procedures
            var selectedProcedures = await _selectedProcedureRepository.GetAll()
                .Where(x => selectedProcedureIds.Contains(x.Id))
                .ToListAsync();

            foreach (var sp in selectedProcedures)
            {
                sp.IsPaid = true;
                sp.ProcedureReceiptId = entity.Id;
                await _selectedProcedureRepository.UpdateAsync(sp);
            }

            return MapToEntityDto(entity);
        }
        private async Task<string> GenerateProcedureReceiptNoAsync(int tenantId)
        {
            return await _numberingService.GenerateReceiptNumberAsync(
                Repository,             // ProcedureReceipt repository
                "PR-REC",               // prefix (Procedure Receipt)
                tenantId,
                "ReceiptNumber"         // property name in ProcedureReceipt entity
            );
        }

        // Card -> webhook success पर call होगा
        public async Task<ProcedureReceiptDto> CreateProcedureReceiptFromStripeAsync(CreateUpdateProcedureReceiptDto input, long[] selectedProcedureIds, string paymentIntentId)
        {
            var entity = ObjectMapper.Map<ProcedureReceipt>(input);
            entity.ReceiptNumber = await GenerateProcedureReceiptNoAsync(input.TenantId);
            entity.Status = InvoiceStatus.Paid;
            entity.PaymentIntentId = paymentIntentId;

            await Repository.InsertAndGetIdAsync(entity);

            // update selected emergency procedures
            var selectedProcedures = await _selectedProcedureRepository.GetAll()
                .Where(x => selectedProcedureIds.Contains(x.Id))
                .ToListAsync();

            foreach (var sp in selectedProcedures)
            {
                sp.IsPaid = true;
                sp.ProcedureReceiptId = entity.Id;
                await _selectedProcedureRepository.UpdateAsync(sp);
            }

            return MapToEntityDto(entity);
        }
        public async Task<string> CreateStripeCheckoutSession(CreateProcedureReceiptWithIdsDto dto)
        {
            var input = dto.Input;
            var selectedProcedureIds = dto.SelectedProcedureIds;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = "http://localhost:4200/app/procedure/procedure-receipts",
                CancelUrl = "http://localhost:4200/app/procedure/procedure-receipts",
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = input.TotalFee * 100, // convert to cents
                    Currency = "inr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Emergency Procedures Charges"
                    },
                },
                Quantity = 1
            }
        },
                Metadata = new Dictionary<string, string>
        {
            { "purpose", "procedure" },
            { "tenantId", input.TenantId.ToString() },
            { "patientId", input.PatientId.ToString() },
            { "totalFee", input.TotalFee.ToString() },
            { "selectedProcedureIds", JsonConvert.SerializeObject(selectedProcedureIds) }
        }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url; // frontend redirect
        }

    }
}
