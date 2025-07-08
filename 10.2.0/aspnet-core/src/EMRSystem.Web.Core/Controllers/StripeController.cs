using Abp.AspNetCore.Mvc.Controllers;
using EMRSystem.Invoice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe.Checkout;

namespace EMRSystem.Controllers
{
    [Route("api/stripe")]
    public class StripeController : AbpController
    {
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IConfiguration _configuration;

        public StripeController(IInvoiceAppService invoiceAppService,IConfiguration configuration)
        {
            _invoiceAppService = invoiceAppService;
            _configuration = configuration;
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]
                );

                // Handle successful payment (using string constants instead of Events class)
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    // Verify the payment was successful
                    if (session.PaymentStatus == "paid")
                    {
                        await _invoiceAppService.MarkAsPaid(
                            long.Parse(session.Metadata["invoiceId"])
                        );
                    }
                }
                // Alternatively handle payment_intent.succeeded
                else if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await _invoiceAppService.MarkAsPaid(
                        long.Parse(paymentIntent.Metadata["invoiceId"])
                    );
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
        [HttpPost]
        [Route("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _configuration["Stripe:WebhookSecret"]
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var invoiceId = long.Parse(session.Metadata["invoiceId"]);

                    // Update invoice status to Paid
                    //await MarkAsPaid(invoiceId);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Logger.Error("Stripe webhook error", e);
                return BadRequest();
            }
        }
    }

    public class CreatePaymentIntentDto
    {
        public decimal Amount { get; set; }
        public long InvoiceId { get; set; }
    }
}
