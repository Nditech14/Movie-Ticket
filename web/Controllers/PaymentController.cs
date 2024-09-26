using Application.Service.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePaymentForUserAsync()
        {
            var result = await _paymentService.CreatePaymentForUserAsync();

            if (result.Success)
            {
                return Ok(new
                {
                    redirectUrl = result.RedirectUrl,
                    message = result.Message
                });
            }

            return BadRequest(new { message = result.Message });
        }

        [HttpGet("verify-payment")]
        public async Task<IActionResult> VerifyPaymentAsync(string reference)
        {
            var result = await _paymentService.VerifyPaymentAsync(reference);

            if (result.Success)
            {
                return Ok(new
                {
                    Reference = result.TransactionId,
                    amountPaid = result.AmountPaid,
                    purchasedMovies = result.PurchasedMovies,
                    message = result.Message
                });
            }

            return BadRequest(new { message = result.Message });
        }
    }
}
