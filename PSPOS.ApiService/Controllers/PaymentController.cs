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
        Console.WriteLine("I Have reached the CreatePaymentIntent method");
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = request.Currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var paymentIntent = paymentIntentService.Create(options);
            Console.WriteLine("Client secret:" + paymentIntent.ClientSecret);
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
}

public class CreatePaymentIntentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
}

public class ProcessPaymentRequest
{
    public string PaymentIntentId { get; set; }
    public string PaymentMethodId { get; set; }
}