using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using EMRSystem.AppointmentReceipt.Dto;
using EMRSystem.Appointments;
using EMRSystem.Deposit;
using EMRSystem.Invoices;
using EMRSystem.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IRepository<EMRSystem.Deposit.Deposit, long> _depositRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAppointmentAppService _appointmentAppService;
        private readonly IRepository<Appointment, long> _appointmentRepository;


        public StripeController(
            IConfiguration configuration,
            IRepository<EMRSystem.Deposit.Deposit, long> depositRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAppointmentAppService appointmentAppService,
            IRepository<Appointment, long> appointmentRepository)
        {
            _configuration = configuration;
            _depositRepository = depositRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _appointmentAppService = appointmentAppService;
            _appointmentRepository = appointmentRepository;
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

                    // Handle deposit payments
                    if (session.Metadata.TryGetValue("purpose", out var purpose) && purpose == "deposit")
                    {
                        if (!session.Metadata.TryGetValue("patientId", out var patientIdStr) ||
                            !session.Metadata.TryGetValue("amount", out var amountStr))
                            return Ok();

                        var patientId = long.Parse(patientIdStr);
                        var tenantId = int.Parse(session.Metadata["tenantId"]);
                        var amount = decimal.Parse(amountStr);
                        var billingMethod = Enum.Parse<BillingMethod>(session.Metadata["billingMethod"]);
                        var depositDateTime = DateTime.Parse(session.Metadata["depositDateTime"]);

                        using (var uow = _unitOfWorkManager.Begin())
                        {
                            var deposit = new EMRSystem.Deposit.Deposit
                            {
                                TenantId = tenantId,
                                PatientId = patientId,
                                Amount = amount,
                                PaymentMethod = PaymentMethod.Card,
                                BillingMethod = billingMethod,
                                DepositDateTime = depositDateTime
                            };

                            await _depositRepository.InsertAsync(deposit);
                            await uow.CompleteAsync();
                        }
                    }
                    // Handle appointment payments
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
                            await _appointmentAppService.GenerateAppointmentReceipt(appointmentId, PaymentMethod.Card.ToString());
                            await uow.CompleteAsync();
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