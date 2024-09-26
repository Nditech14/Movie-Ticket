using Application.Dto;
using Application.Service.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CinemaController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;
        private readonly IMovieService _movieService;

        public CinemaController(ICinemaService cinemaService, IMovieService movieService)
        {
            _cinemaService = cinemaService;
            _movieService = movieService;
        }

        [HttpGet("get-cinema/{id}")]
        public async Task<IActionResult> GetCinemaById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Cinema ID cannot be empty.");

            var result = await _cinemaService.GetCinemaByIdAsync(id);
            if (result.IsSuccessful)
                return Ok(result.Data);

            return StatusCode(result.Code, result.Errors);
        }

        [HttpGet("get-all-cinema")]
        public async Task<IActionResult> GetAllCinemas()
        {
            var cinemas = await _cinemaService.GetAllCinemasAsync();
            return Ok(cinemas);
        }

        [HttpPatch("update-cinema-name/{id}")]
        public async Task<IActionResult> UpdateCinemaName(string id, [FromBody] CinemaDto cinemaDto)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Cinema ID cannot be empty.");

            if (cinemaDto == null || string.IsNullOrEmpty(cinemaDto.CinemaName))
                return BadRequest("Cinema name is required.");

            try
            {
                var updatedCinema = await _cinemaService.UpdateCinemaNameAsync(id, cinemaDto);
                await _movieService.UpdateMoviesWithCinemaNameAsync(id, cinemaDto.CinemaName);

                return Ok(updatedCinema);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCinema(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Cinema ID cannot be empty.");

            var isDeleted = await _cinemaService.DeleteCinemaAsync(id);
            if (isDeleted)
                return NoContent();

            return NotFound($"Cinema with ID '{id}' not found.");
        }
    }
}
