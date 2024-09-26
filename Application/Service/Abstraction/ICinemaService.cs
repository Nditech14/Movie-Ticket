using Application.Dto;
using Application.ResponseDto;

namespace Application.Service.Abstraction
{
    public interface ICinemaService
    {
        Task<bool> DeleteCinemaAsync(string id);
        Task<IEnumerable<CinemaDto>> GetAllCinemasAsync();
        Task<ResponseDto<CinemaDto>> GetCinemaByIdAsync(string id);
        Task<(IEnumerable<CinemaDto> Items, string ContinuationToken)> GetCinemasWithPaginationAsync(string continuationToken, int maxItemCount = 30);
        Task<ResponseDto<CinemaDto>> UpdateCinemaNameAsync(string id, CinemaDto cinemaDto);
    }
}