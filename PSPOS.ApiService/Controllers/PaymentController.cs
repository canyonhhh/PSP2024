using Microsoft.AspNetCore.Mvc;
using Serilog;
using Stripe;

namespace PSPOS.ApiService.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{

    [HttpPost("create-intent")]
    public IActionResult CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        Log.Information("Creating payment intent for amount: {Amount}, currency: {Currency}", request.Amount, request.Currency);
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
            Log.Information("Payment intent created successfully with client secret: {ClientSecret}", paymentIntent.ClientSecret);
            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }
        catch (StripeException ex)
        {
            Log.Error("StripeException occurred while creating payment intent. Error: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("process")]
    public IActionResult ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        Log.Information("Processing payment for PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Confirm(request.PaymentIntentId, new PaymentIntentConfirmOptions
            {
                PaymentMethod = request.PaymentMethodId
            });

            if (paymentIntent.Status == "succeeded")
            {
                Log.Information("Payment succeeded for PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);
                return Ok(new { success = true, message = "Payment successful!" });
            }
            Log.Warning("Payment failed for PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);
            return BadRequest(new { success = false, message = "Payment failed." });
        }
        catch (StripeException ex)
        {
            Log.Error("StripeException occurred while processing payment. Error: {Message}", ex.Message);
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