using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using AutoMapper;
using Core.Entities;
using System.Net;

namespace Application.Service.Implementation
{
    public class CinemaService : ICinemaService
    {
        private readonly ICosmosDbService<Cinema> _cinemaDbService;
        private readonly IMapper _mapper;

        public CinemaService(
            ICosmosDbService<Cinema> cinemaDbService,
            IMapper mapper)
        {
            _cinemaDbService = cinemaDbService;
            _mapper = mapper;
        }

        // Get Cinema by ID
        public async Task<ResponseDto<CinemaDto>> GetCinemaByIdAsync(string id)
        {
            try
            {
                var cinema = await _cinemaDbService.GetItemAsync(id, id);
                if (cinema == null)
                {
                    var errors = new List<Error> { new Error("NotFound", $"Cinema with ID '{id}' not found.") };
                    return ResponseDto<CinemaDto>.Failure(errors, (int)HttpStatusCode.NotFound);
                }

                var cinemaDto = _mapper.Map<CinemaDto>(cinema);
                return ResponseDto<CinemaDto>.Success(cinemaDto, "Cinema retrieved successfully.");
            }
            catch (Exception ex)
            {
                var errors = new List<Error> { new Error("GetCinemaError", ex.Message) };
                return ResponseDto<CinemaDto>.Failure(errors, (int)HttpStatusCode.InternalServerError);
            }
        }

        // Get All Cinemas
        public async Task<IEnumerable<CinemaDto>> GetAllCinemasAsync()
        {
            var cinemas = await _cinemaDbService.GetItemsAsync("SELECT * FROM c");
            return _mapper.Map<IEnumerable<CinemaDto>>(cinemas);
        }

        // Update Cinema Name
        public async Task<ResponseDto<CinemaDto>> UpdateCinemaNameAsync(string id, CinemaDto cinemaDto)
        {
            try
            {
                var existingCinema = await _cinemaDbService.GetItemAsync(id, id);
                if (existingCinema == null)
                {
                    var errors = new List<Error> { new Error("NotFound", $"Cinema with ID '{id}' not found.") };
                    return ResponseDto<CinemaDto>.Failure(errors, (int)HttpStatusCode.NotFound);
                }

                existingCinema.CinemaName = cinemaDto.CinemaName;

                await _cinemaDbService.UpdateItemAsync(existingCinema.Id, existingCinema, existingCinema.Id);

                var updatedCinemaDto = _mapper.Map<CinemaDto>(existingCinema);
                return ResponseDto<CinemaDto>.Success(updatedCinemaDto, "Cinema updated successfully.");
            }
            catch (Exception ex)
            {
                var errors = new List<Error> { new Error("UpdateCinemaError", ex.Message) };
                return ResponseDto<CinemaDto>.Failure(errors, (int)HttpStatusCode.InternalServerError);
            }
        }

        // Delete Cinema by ID
        public async Task<bool> DeleteCinemaAsync(string id)
        {
            var existingCinema = await _cinemaDbService.GetItemAsync(id, id);
            if (existingCinema == null)
            {
                return false;
            }

            await _cinemaDbService.DeleteItemAsync(id, id);
            return true;
        }

        // Paginated retrieval of Cinemas
        public async Task<(IEnumerable<CinemaDto> Items, string ContinuationToken)> GetCinemasWithPaginationAsync(string continuationToken, int maxItemCount = 30)
        {
            var (cinemas, newContinuationToken) = await _cinemaDbService.GetItemsWithContinuationTokenAsync(continuationToken, maxItemCount);
            var cinemaDtos = _mapper.Map<IEnumerable<CinemaDto>>(cinemas);
            return (cinemaDtos, newContinuationToken);
        }
    }
}
