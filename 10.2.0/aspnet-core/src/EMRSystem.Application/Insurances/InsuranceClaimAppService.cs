using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using EMRSystem.Insurances.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances
{
    public class InsuranceClaimAppService : AsyncCrudAppService<
            InsuranceClaim, InsuranceClaimDto, long, PagedInsuranceClaimDto,
            CreateUpdateInsuranceClaimDto, CreateUpdateInsuranceClaimDto>,
        IInsuranceClaimAppService
    {
        private readonly IRepository<InsuranceClaim, long> _repo;
        private readonly IRepository<EMRSystem.Invoices.Invoice, long> _invoiceRepo;
        // optional: patientInsurance repo if you want validations
        private readonly IRepository<PatientInsurance, long> _patientInsuranceRepo;

        public InsuranceClaimAppService(
            IRepository<InsuranceClaim, long> repository,
            IRepository<EMRSystem.Invoices.Invoice, long> invoiceRepo,
            IRepository<PatientInsurance, long> patientInsuranceRepo
            ) : base(repository)
        {
            _repo = repository;
            _invoiceRepo = invoiceRepo;
            _patientInsuranceRepo = patientInsuranceRepo;
        }
        protected override IQueryable<InsuranceClaim> CreateFilteredQuery(PagedInsuranceClaimDto input)
        {
            // Start with base query including relationships
            var query = _repo.GetAll()
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.Patient)
                .Include(x => x.PatientInsurance)
                    .ThenInclude(pi => pi.InsuranceMaster)
                .AsQueryable(); // <-- ensure final return type is IQueryable

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x =>
                    (x.Invoice.InvoiceNo != null && x.Invoice.InvoiceNo.Contains(input.Keyword)) ||
                    (x.Invoice.Patient.FullName != null && x.Invoice.Patient.FullName.Contains(input.Keyword)) ||
                    (x.PatientInsurance.InsuranceMaster.InsuranceName != null && x.PatientInsurance.InsuranceMaster.InsuranceName.Contains(input.Keyword))
                );
            }

            return query;
        }


        // override create to add validation / link with invoice
        public override async Task<InsuranceClaimDto> CreateAsync(CreateUpdateInsuranceClaimDto input)
        {
            // validate invoice exists
            var invoice = await _invoiceRepo.FirstOrDefaultAsync(input.InvoiceId);
            if (invoice == null)
                throw new UserFriendlyException("Invoice not found.");

            // optionally check duplicate claim for same invoice
            var existing = await _repo.GetAll().FirstOrDefaultAsync(x => x.InvoiceId == input.InvoiceId);
            if (existing != null)
                throw new UserFriendlyException("Insurance claim for this invoice already exists.");

            var entity = ObjectMapper.Map<InsuranceClaim>(input);
            entity.CreatedDate = DateTime.Now; // FullAuditedEntity will set CreatedOn, but explicit if needed
            entity.Status = ClaimStatus.Pending;
            await _repo.InsertAsync(entity);

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        // helper to fetch by invoice id
        public async Task<InsuranceClaimDto> GetByInvoiceIdAsync(long invoiceId)
        {
            var data = await _repo.GetAll()
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.Patient)
                .Include(x => x.PatientInsurance)
                    .ThenInclude(pi => pi.InsuranceMaster)
                .Where(x => x.InvoiceId == invoiceId)
                .Select(x => new InsuranceClaimDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    InvoiceId = x.InvoiceId,
                    PatientInsuranceId = x.PatientInsuranceId,
                    TotalAmount = x.TotalAmount,
                    AmountPayByInsurance = x.AmountPayByInsurance,
                    AmountPayByPatient = x.AmountPayByPatient,
                    Remarks = x.Remarks,
                    Status = x.Status,
                    SubmittedOn = x.SubmittedOn,
                    RespondedOn = x.RespondedOn,
                    PaidOn = x.PaidOn
                })
                .FirstOrDefaultAsync();

            return data;
        }

        // mark claim as submitted
        public async Task SubmitClaimAsync(long claimId)
        {
            var claim = await _repo.GetAsync(claimId);
            claim.Status = ClaimStatus.Submitted;
            claim.SubmittedOn = DateTime.Now;
            await _repo.UpdateAsync(claim);
        }

        // mark as paid - record paid amount and date; optionally update invoice status
        public async Task MarkAsPaidAsync(EntityDto<long> input, decimal paidAmount)
        {
            var claim = await _repo.GetAsync(input.Id);
            claim.AmountPayByInsurance = paidAmount;
            claim.PaidOn = DateTime.Now;
            claim.Status = ClaimStatus.Paid;
            await _repo.UpdateAsync(claim);

            // optionally: update linked invoice status to Paid/PartiallyPaid based on paid amounts
            var invoice = await _invoiceRepo.FirstOrDefaultAsync(claim.InvoiceId);
            if (invoice != null)
            {
                // simple example: if insurance paid covers full invoice.TotalAmount, mark Paid
                if (paidAmount >= invoice.TotalAmount)
                {
                    invoice.Status = EMRSystem.Invoices.InvoiceStatus.Paid;
                }
                else
                {
                    invoice.Status = EMRSystem.Invoices.InvoiceStatus.Unpaid; // or PartiallyPaid if you add enum
                }
                await _invoiceRepo.UpdateAsync(invoice);
            }
        }


        public override async Task<InsuranceClaimDto> UpdateAsync(CreateUpdateInsuranceClaimDto input)
        {
                var claim = await _repo.GetAll()
                                       .Include(x => x.Invoice)
                                           .ThenInclude(i => i.Items) // Now works
                                       .Include(x => x.PatientInsurance)
                                           .ThenInclude(pi => pi.InsuranceMaster)
                                       .FirstOrDefaultAsync(x => x.Id == input.Id);

                if (claim == null)
                    throw new UserFriendlyException("Insurance claim not found.");

                // Map basic fields
                claim.AmountPayByInsurance = input.AmountPayByInsurance;
                claim.AmountPayByPatient = input.AmountPayByPatient;
                claim.Status = input.Status;

                if (input.Status == ClaimStatus.PartialApproved || input.Status == ClaimStatus.Approved)
                    claim.RespondedOn = DateTime.Now;


                // Update Invoice Items based on DTO
                foreach (var itemDto in input.Items)
                {
                    var invoiceItem = claim.Invoice.Items.FirstOrDefault(x => x.Id == itemDto.Id);
                    if (invoiceItem != null)
                    {
                        invoiceItem.IsCoveredByInsurance = itemDto.IsCoveredByInsurance;
                        invoiceItem.ApprovedAmount = itemDto.ApprovedAmount;
                        invoiceItem.NotApprovedAmount = itemDto.NotApprovedAmount;
                    }
                }

                // Update Invoice totals
                claim.Invoice.ApprovedAmount = input.AmountPayByInsurance;
                claim.Invoice.CoPayAmount = input.AmountPayByPatient;

                await CurrentUnitOfWork.SaveChangesAsync(); // Saves both claim & invoice & items

                return MapToEntityDto(claim);

        }

    }
}
