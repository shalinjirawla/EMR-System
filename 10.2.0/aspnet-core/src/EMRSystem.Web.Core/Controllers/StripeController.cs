using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using EMRSystem.AppointmentReceipt.Dto;
using EMRSystem.Appointments;
using EMRSystem.Deposit;
using EMRSystem.EmergencyProcedure;
using EMRSystem.Invoices;
using EMRSystem.LabTestReceipt;
using EMRSystem.LabTestReceipt.Dto;
using EMRSystem.Patients;
using EMRSystem.Pharmacist;
using EMRSystem.Pharmacist.Dto;
using EMRSystem.PrescriptionLabTest;
using EMRSystem.ProcedureReceipts;
using EMRSystem.ProcedureReceipts.Dto;
using EMRSystem.TempStripeData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PaymentMethod = EMRSystem.Invoices.PaymentMethod;

namespace EMRSystem.Controllers
{
    [AllowAnonymous]
    [Route("api/stripe/webhook")]
    [ApiController]
    public class StripeController : AbpController
    {
        private readonly IConfiguration _configuration;
        //private readonly IRepository<EMRSystem.Deposit.Deposit, long> _depositRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAppointmentAppService _appointmentAppService;
        private readonly IProcedureReceiptAppService _procedureReceiptAppService;
        private readonly IRepository<Appointment, long> _appointmentRepository;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        private readonly ILabTestReceiptAppService _labTestReceiptAppService;
        private readonly IDepositTransactionAppService _depositetansactionAppService;
        private readonly IRepository<DepositTransaction, long> _depositTransactionRepository;
        private readonly IRepository<PatientDeposit, long> _patientDepositRepository;
        private readonly IRepository<SelectedEmergencyProcedures, long> _selectedProcedureRepository;
        private readonly IPharmacistPrescriptionsAppService _pharmacistPrescriptionsAppService;
        private readonly ITempStripeDataService _tempStripeDataService;


        public StripeController(
            IConfiguration configuration,
            //IRepository<EMRSystem.Deposit.Deposit, long> depositRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAppointmentAppService appointmentAppService,
            IProcedureReceiptAppService procedureReceiptAppService,
            IPharmacistPrescriptionsAppService pharmacistPrescriptionsAppService,
            ILabTestReceiptAppService labTestReceiptAppService,
            IRepository<DepositTransaction,long> depositTransactionRepository,
            ITempStripeDataService tempStripeDataService,
            IRepository<SelectedEmergencyProcedures, long> selectedProcedureRepository,
            IRepository<PatientDeposit, long> patientDepositRepository,
            IDepositTransactionAppService depositetansactionAppService,
            IRepository<Appointment, long> appointmentRepository,
            IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository)
        {
            _configuration = configuration;
            //_depositRepository = depositRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _labTestReceiptAppService = labTestReceiptAppService;
            _appointmentAppService = appointmentAppService;
            _selectedProcedureRepository = selectedProcedureRepository;
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _tempStripeDataService = tempStripeDataService;
            _patientDepositRepository = patientDepositRepository;
            _appointmentRepository = appointmentRepository;
            _depositetansactionAppService = depositetansactionAppService;
            _procedureReceiptAppService = procedureReceiptAppService;
            _pharmacistPrescriptionsAppService = pharmacistPrescriptionsAppService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();
            if (string.IsNullOrEmpty(stripeSignature))
            {
                Logger.Warn("Stripe signature missing in webhook.");
                return BadRequest("Missing Stripe signature.");
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _configuration["Stripe:WebhookSecret"],
                    throwOnApiVersionMismatch: false);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    if (session.Metadata == null)
                        return Ok();
                    else if (session.Metadata.TryGetValue("purpose", out var appointmentPurpose) && appointmentPurpose == "appointment")
                    {
                        if (!session.Metadata.TryGetValue("appointmentId", out var appointmentIdStr))
                            return Ok();

                        var appointmentId = long.Parse(appointmentIdStr);
                        using (var uow = _unitOfWorkManager.Begin())
                        {
                            var appointment = await _appointmentRepository.GetAsync(appointmentId);
                            appointment.IsPaid = true;
                            await _appointmentRepository.UpdateAsync(appointment);
                            await _appointmentAppService.GenerateAppointmentReceipt(appointmentId, PaymentMethod.Card.ToString(), session.PaymentIntentId);
                            await uow.CompleteAsync();
                        }
                    }
                    else if (session.Metadata.TryGetValue("purpose", out var labTestpurpose) && labTestpurpose == "labTest")
                    {
                        if (!session.Metadata.TryGetValue("labTestReceiptDtoJson", out var dtoJson))
                            return Ok();

                        var dto = JsonConvert.DeserializeObject<CreateLabTestReceiptDto>(dtoJson);

                        // Tenant ID ensure kar le, kyunki webhook anonymous request hai
                        if (session.Metadata.TryGetValue("tenantId", out var tenantIdStr) && int.TryParse(tenantIdStr, out var tenantId))
                        {
                            dto.TenantId = tenantId;
                        }

                        using (var uow = _unitOfWorkManager.Begin())
                        {
                            dto.PaymentIntentId = session.PaymentIntentId;
                            await _labTestReceiptAppService.CreateLabTestReceipt(dto);
                            await uow.CompleteAsync(); // 🔹 DB commit confirm karega
                        }
                    }
                    else if (session.Metadata.TryGetValue("purpose", out var procedurePurpose) && procedurePurpose == "procedure")
                    {
                        try
                        {
                            Logger.Info("Starting procedure receipt creation from webhook...");

                            if (!session.Metadata.TryGetValue("patientId", out var patientIdStr) ||
                                !session.Metadata.TryGetValue("totalFee", out var totalFeeStr) ||
                                !session.Metadata.TryGetValue("selectedProcedureIds", out var idsJson))
                            {
                                Logger.Warn("Procedure webhook metadata missing");
                                return Ok();
                            }

                            var patientId = long.Parse(patientIdStr);
                            var totalFee = decimal.Parse(totalFeeStr);
                            var selectedProcedureIds = JsonConvert.DeserializeObject<long[]>(idsJson);

                            int tenantId = 0;
                            if (session.Metadata.TryGetValue("tenantId", out var tenantIdStr) && int.TryParse(tenantIdStr, out var tId))
                            {
                                tenantId = tId;
                            }

                            Logger.Info($"Metadata parsed: PatientId={patientId}, TenantId={tenantId}, TotalFee={totalFee}, Procedures={string.Join(",", selectedProcedureIds)}");

                            var dto = new CreateUpdateProcedureReceiptDto
                            {
                                TenantId = tenantId,
                                PatientId = patientId,
                                TotalFee = totalFee,
                                PaymentMethod = PaymentMethod.Card,
                                Status = InvoiceStatus.Paid,
                                PaymentDate = DateTime.Now
                            };

                            using (var uow = _unitOfWorkManager.Begin())
                            {
                                await _procedureReceiptAppService.CreateProcedureReceiptFromStripeAsync(dto, selectedProcedureIds, session.PaymentIntentId);
                                await uow.CompleteAsync();
                            }

                            Logger.Info("Procedure receipt created successfully!");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Procedure webhook failed", ex);
                            return StatusCode(500);
                        }
                    }
                    else if (session.Metadata.TryGetValue("purpose", out var depositPurpose) && depositPurpose == "deposit")
                    {
                        if (!session.Metadata.TryGetValue("patientDepositId", out var depositIdStr) ||
                            !session.Metadata.TryGetValue("amount", out var amountStr))
                            return Ok();

                        var patientDepositId = long.Parse(depositIdStr);
                        var amount = decimal.Parse(amountStr);

                        int? tenantId = null;
                        if (session.Metadata.TryGetValue("tenantId", out var tenantIdStr) && int.TryParse(tenantIdStr, out var tId))
                        {
                            tenantId = tId;
                        }

                        using (var uow = _unitOfWorkManager.Begin())
                        {
                            var transaction = new DepositTransaction
                            {
                                PatientDepositId = patientDepositId,
                                TenantId = tenantId ?? 0,
                                Amount = amount,
                                PaymentMethod = PaymentMethod.Card,
                                IsPaid = true,
                                Description = "Credit Deposit.",
                                TransactionDate = DateTime.Now,
                                TransactionType = TransactionType.Credit,
                                PaymentIntentId = session.PaymentIntentId

                            };
                            transaction.ReceiptNo = await _depositetansactionAppService.GenerateReceiptNoAsync(tenantId ?? 0);

                            await _depositTransactionRepository.InsertAsync(transaction);

                            var patientDeposit = await _patientDepositRepository.GetAsync(patientDepositId);
                            patientDeposit.TotalCreditAmount += amount;
                            patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

                            await _patientDepositRepository.UpdateAsync(patientDeposit);

                            await uow.CompleteAsync();
                        }
                    }
                    else if (session.Metadata.TryGetValue("purpose", out var pharmacistPurpose) && pharmacistPurpose == "pharmacistPrescription")
                    {
                        if (!session.Metadata.TryGetValue("tempDataId", out var tempDataId))
                            return Ok();
                        try
                        {
                            var dto = await _tempStripeDataService.RetrieveData(tempDataId);
                            if (dto == null)
                            {
                                Logger.Warn($"Temp data not found for ID: {tempDataId}");
                                return Ok();
                            }
                            await _tempStripeDataService.RemoveData(tempDataId);
                            if (session.Metadata.TryGetValue("tenantId", out var tenantIdStr) &&
                            int.TryParse(tenantIdStr, out var tenantId))
                            {
                                dto.pharmacistPrescriptionsDto.TenantId = tenantId;
                            }
                            using (var uow = _unitOfWorkManager.Begin())
                            {
                                dto.pharmacistPrescriptionsDto.PaymentIntentId = session.PaymentIntentId;
                                dto.pharmacistPrescriptionsDto.PaymentMethod = PaymentMethod.Card;
                                dto.pharmacistPrescriptionsDto.IsPaid = true;
                                await _pharmacistPrescriptionsAppService.CreatePharmacistPrescriptionsWithItem(dto);
                                await uow.CompleteAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Error processing pharmacist prescription webhook: {ex.Message}", ex);
                            // Even if error, don't keep stale data
                            await _tempStripeDataService.RemoveData(tempDataId);
                            throw;
                        }
                    }
                   

                }

                return Ok();
            }
            catch (StripeException e)
            {
                Logger.Error("Stripe webhook error", e);
                return BadRequest();
            }
            catch (Exception e)
            {
                Logger.Error("Webhook processing error", e);
                return StatusCode(500);
            }
        }
    }
}