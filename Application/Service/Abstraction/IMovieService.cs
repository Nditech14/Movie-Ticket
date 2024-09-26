using Application.Dto;
using Application.ResponseDto;

namespace Application.Service.Abstraction
{
    public interface IMovieService
    {
        Task<ResponseDto<MovieResponseDto>> AddMovieAsync(MovieCreationDto movieDto);
        Task<bool> DeleteMovieAsync(string id);
        Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync();
        Task<MovieResponseDto> GetMovieByIdAsync(string id);
        Task<(IEnumerable<MovieResponseDto> Items, string ContinuationToken)> GetMoviesWithPaginationAsync(string continuationToken, int maxItemCount = 30);
        Task<ResponseDto<MovieResponseDto>> SearchMovieWithName(MovieDto movieDto);
        Task<MovieResponseDto> UpdateMovieImageAsync(string id, MovieImageUploadDto updatemovieDto);
        Task<MovieResponseDto> UpdateMovieStatusAsync(string id, UpdateMovieStatusDto newStatusDto);
        Task UpdateMoviesWithActorImageAsync(string actorId, string newImageUrl);
        Task UpdateMoviesWithCinemaNameAsync(string cinemaId, string newCinemaName);
        Task UpdateMoviesWithProducerImageAsync(string producerId, string newImageUrl);
    }
}