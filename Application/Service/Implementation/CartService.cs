using Application.ResponseDto;
using Application.Service.Abstraction;
using Core.Entities;
using Core.Enum;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace Application.Service.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICosmosDbService<Cart> _cartDbService;
        private readonly ICosmosDbService<Movie> _movieDbService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(
            ICosmosDbService<Cart> cartDbService,
            ICosmosDbService<Movie> movieDbService,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartDbService = cartDbService;
            _movieDbService = movieDbService;
            _httpContextAccessor = httpContextAccessor;
        }


        private string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User is not authenticated.");
            }
            return userId;
        }


        private async Task<Cart> GetOrCreateCartForUserAsync(string userId)
        {
            var cartId = _httpContextAccessor.HttpContext.Session.GetString("CartId");
            Cart cart = null;

            if (!string.IsNullOrEmpty(cartId))
            {
                cart = await _cartDbService.GetItemAsync(cartId, userId);
            }
            else
            {
                cart = await _cartDbService.GetItemAsync(userId, userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = Guid.NewGuid().ToString(),
                        userId = userId,
                        CartItems = new List<CartItem>()
                    };


                    await _cartDbService.AddItemAsync(cart, userId);
                    _httpContextAccessor.HttpContext.Session.SetString("CartId", cart.Id);
                }
                else
                {

                    _httpContextAccessor.HttpContext.Session.SetString("CartId", cart.Id);
                }
            }

            return cart;
        }


        public async Task<ResponseDto<Cart>> GetOrCreateCartAsync()
        {
            var userId = GetUserId();
            var cart = await GetOrCreateCartForUserAsync(userId);

            return ResponseDto<Cart>.Success(cart, "Cart ready for operations.");
        }


        public async Task<ResponseDto<Cart>> AddToCartAsync(string movieId, int quantity)
        {
            var userId = GetUserId();


            var movie = await _movieDbService.GetItemAsyncz(movieId);
            if (movie == null)
            {
                return ResponseDto<Cart>.Failure(
                    new List<Error> { new Error("MovieNotFound", "The specified movie does not exist.") },
                    (int)HttpStatusCode.NotFound
                );
            }

            if (movie.Status == MovieStatus.Expired)
            {
                return ResponseDto<Cart>.Failure(
                    new List<Error> { new Error("ExpiredMovie", "The movie is expired and cannot be added to the cart.") },
                    (int)HttpStatusCode.BadRequest
                );
            }

            var cart = await GetOrCreateCartForUserAsync(userId);


            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MovieId == movieId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    MovieId = movie.Id,
                    MovieTitle = movie.Title,
                    Price = movie.Price,
                    Quantity = quantity,
                    userId = userId
                });
            }

            await _cartDbService.UpdateItemAsync(cart.Id, cart, userId);

            return ResponseDto<Cart>.Success(cart, "Item added to cart successfully.");
        }


        public async Task<ResponseDto<Cart>> RemoveFromCartAsync(string movieId)
        {
            var userId = GetUserId();
            var cart = await GetOrCreateCartForUserAsync(userId);


            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MovieId == movieId);
            if (cartItem == null)
            {
                return ResponseDto<Cart>.Failure(
                    new List<Error> { new Error("ItemNotFound", "The movie is not in the cart.") },
                    (int)HttpStatusCode.NotFound
                );
            }

            cart.CartItems.Remove(cartItem);


            await _cartDbService.UpdateItemAsync(cart.Id, cart, userId);

            return ResponseDto<Cart>.Success(cart, "Item removed from cart successfully.");
        }


        public async Task<ResponseDto<Cart>> GetCartAsync()
        {
            var userId = GetUserId();
            var cartId = _httpContextAccessor.HttpContext.Session.GetString("CartId");


            var cart = await _cartDbService.GetItemAsync(cartId, userId);
            if (cart == null)
            {
                return ResponseDto<Cart>.Failure(
                    new List<Error> { new Error("CartNotFound", "No cart found for the user.") },
                    (int)HttpStatusCode.NotFound
                );
            }

            return ResponseDto<Cart>.Success(cart, "Cart retrieved successfully.");
        }
    }
}








