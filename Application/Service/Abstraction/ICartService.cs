using Application.ResponseDto;
using Core.Entities;

namespace Application.Service.Abstraction
{
    public interface ICartService
    {
        Task<ResponseDto<Cart>> AddToCartAsync(string movieId, int quantity);
        Task<ResponseDto<Cart>> GetCartAsync();
        Task<ResponseDto<Cart>> GetOrCreateCartAsync();
        Task<ResponseDto<Cart>> RemoveFromCartAsync(string movieId);
    }
}