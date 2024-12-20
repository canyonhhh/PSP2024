using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace PSPOS.ApiService.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{

    [HttpPost("create-intent")]
    public IActionResult CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount),
                Currency = request.Currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var paymentIntent = paymentIntentService.Create(options);
            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }
        catch (StripeException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("process")]
    public IActionResult ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Confirm(request.PaymentIntentId, new PaymentIntentConfirmOptions
            {
                PaymentMethod = request.PaymentMethodId
            });

            if (paymentIntent.Status == "succeeded")
            {
                return Ok(new { success = true, message = "Payment successful!" });
            }
            return BadRequest(new { success = false, message = "Payment failed." });
        }
        catch (StripeException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }



    [HttpPost("refund")]
    public IActionResult RefundPayment([FromBody] RefundRequest request)
    {
        try
        {
            var refundService = new RefundService();
            var options = new RefundCreateOptions
            {
                PaymentIntent = request.ExternalTransactionId,
            };

            var refund = refundService.Create(options);
            return Ok(new { success = true, message = "Refund successful!", refundId = refund.Id });
        }
        catch (StripeException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreatePaymentIntentRequest
{
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
}

public class ProcessPaymentRequest
{
    public required string PaymentIntentId { get; set; }
    public required string PaymentMethodId { get; set; }
}

public class RefundRequest
{
    public required string ExternalTransactionId { get; set; }
}