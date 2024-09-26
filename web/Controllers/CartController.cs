using Application.Service.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(string movieId, int quantity)
        {
            try
            {
                var result = await _cartService.AddToCartAsync(movieId, quantity);
                if (result.Code == (int)HttpStatusCode.OK)
                    return Ok(result);

                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ex.Message });
            }
        }

        [HttpDelete("remove/{movieId}")]
        public async Task<IActionResult> RemoveFromCart(string movieId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(movieId);
                if (result.Code == (int)HttpStatusCode.OK)
                    return Ok(result);

                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var result = await _cartService.GetCartAsync();
                if (result.Code == (int)HttpStatusCode.OK)
                    return Ok(result);

                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { ex.Message });
            }
        }
    }
}
