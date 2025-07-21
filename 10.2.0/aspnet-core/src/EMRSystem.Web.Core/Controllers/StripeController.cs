using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using EMRSystem.Invoice;
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
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Controllers
{
    [AllowAnonymous]
    [Route("api/stripe/webhook")]
    [ApiController]
    public class StripeController : AbpController
    {
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IConfiguration _configuration;
        private readonly IRepository<EMRSystem.Deposit.Deposit, long> _depositRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public StripeController(IInvoiceAppService invoiceAppService,IConfiguration configuration, IRepository<EMRSystem.Deposit.Deposit, long> depositRepository,
    IUnitOfWorkManager unitOfWorkManager)
        {
            _invoiceAppService = invoiceAppService;
            _configuration = configuration;
            _depositRepository = depositRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }


        [HttpPost]
public async Task<IActionResult> HandleWebhook()
{
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();
    
    try
    {
                // 1. Verify event construction
      var stripeEvent = EventUtility.ConstructEvent(
            json,
            stripeSignature,
            _configuration["Stripe:WebhookSecret"],
            throwOnApiVersionMismatch: false
        );



        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            
            // 3. Validate metadata exists
            if (session.Metadata == null)
            {
                return Ok();
            }
            
            if (!session.Metadata.TryGetValue("patientId", out var patientIdStr) ||
                !session.Metadata.TryGetValue("amount", out var amountStr))
            {
                return Ok();
            }

                    // 4. Parse safely
                    var patientId = long.Parse(session.Metadata["patientId"]);
                    var tenantId = int.Parse(session.Metadata["tenantId"]);

                    var amount = decimal.Parse(session.Metadata["amount"]);
                    var billingMethodString = session.Metadata["billingMethod"];
                    var billingMethod = Enum.Parse<BillingMethod>(billingMethodString);
                    var depositDateTimeString = session.Metadata["depositDateTime"];
                    var depositDateTime = DateTime.Parse(depositDateTimeString);

                   

                    // 5. Handle deposit creation
                    using (var uow = _unitOfWorkManager.Begin())
            {
                        var deposit = new EMRSystem.Deposit.Deposit
                        {
                            TenantId = tenantId,
                            PatientId = patientId,
                            Amount = amount,
                            PaymentMethod = EMRSystem.Invoices.PaymentMethod.Card,
                            BillingMethod = billingMethod,
                            DepositDateTime = depositDateTime
                        };

                        await _depositRepository.InsertAsync(deposit);
                await uow.CompleteAsync();
            }
        }
        return Ok();
    }
    catch (StripeException e)
    {
        return BadRequest();
    }
    catch (Exception e)
    {
        return StatusCode(500);
    }
}
        //[HttpPost]
        //[Route("stripe-webhook")]
        //public async Task<IActionResult> StripeWebhook()
        //{
        //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //    var stripeSignature = Request.Headers["Stripe-Signature"];

        //    try
        //    {
        //        var stripeEvent = EventUtility.ConstructEvent(
        //            json,
        //            stripeSignature,
        //            _configuration["Stripe:WebhookSecret"]
        //        );

        //        if (stripeEvent.Type == "checkout.session.completed")
        //        {
        //            var session = stripeEvent.Data.Object as Session;
        //            var invoiceId = long.Parse(session.Metadata["invoiceId"]);

        //            // Update invoice status to Paid
        //            //await MarkAsPaid(invoiceId);
        //        }

        //        return Ok();
        //    }
        //    catch (StripeException e)
        //    {
        //        Logger.Error("Stripe webhook error", e);
        //        return BadRequest();
        //    }
        //}
    }

    public class CreatePaymentIntentDto
    {
        public decimal Amount { get; set; }
        public long InvoiceId { get; set; }
    }
}
