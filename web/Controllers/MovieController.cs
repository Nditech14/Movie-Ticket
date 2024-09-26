using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/movies")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public MovieController(IMovieService movieService, IMapper mapper)
        {
            _movieService = movieService;
            _mapper = mapper;
        }

        [HttpGet("get-movie/{id}")]
        public async Task<IActionResult> GetMovieById(string id)
        {
            var result = await _movieService.GetMovieByIdAsync(id);
            if (result != null)
                return Ok(result);

            return NotFound(new { message = $"Movie with ID '{id}' not found." });
        }

        [HttpDelete("delete-movie/{id}")]
        public async Task<IActionResult> DeleteMovieById(string id)
        {
            var result = await _movieService.DeleteMovieAsync(id);
            if (result)
                return Ok(new { message = $"Movie with ID '{id}' deleted successfully." });

            return NotFound(new { message = $"Movie with ID '{id}' not found." });
        }

        [HttpPost("add-movie")]
        public async Task<IActionResult> AddMovie([FromBody] MovieCreationDto movieDto)
        {
            var result = await _movieService.AddMovieAsync(movieDto);
            if (result.IsSuccessful)
                return Ok(result.Data);

            return StatusCode(result.Code, result.Errors);
        }

        [HttpPut("{id}/update-status")]
        public async Task<ActionResult<MovieResponseDto>> UpdateMovieAvailability(string id, [FromForm] UpdateMovieStatusDto newStatusDto)
        {
            try
            {
                var updatedMovie = await _movieService.UpdateMovieStatusAsync(id, newStatusDto);
                return Ok(updatedMovie);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/update-image")]
        public async Task<IActionResult> UpdateMovieImage(string id, [FromForm] MovieImageUploadDto updateMovieDto)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Movie ID is required.");

            if (updateMovieDto == null || updateMovieDto.ImageFile == null)
                return BadRequest("Image file is required.");

            try
            {
                var updatedMovie = await _movieService.UpdateMovieImageAsync(id, updateMovieDto);
                return Ok(updatedMovie);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("SearchByName")]
        public async Task<IActionResult> SearchMovieByName(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                ModelState.AddModelError("MovieTitle", "Movie title cannot be empty.");
                return BadRequest(ModelState);
            }

            var movieDto = new MovieDto { Title = title };
            var result = await _movieService.SearchMovieWithName(movieDto);

            if (!result.IsSuccessful)
                return StatusCode(result.Code, result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("all-movies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var result = await _movieService.GetAllMoviesAsync();
            var responseDto = _mapper.Map<List<ResponseMoviesDto>>(result);
            return Ok(responseDto);
        }

        [HttpGet("load-more-movies")]
        public async Task<IActionResult> LoadMoreMovies([FromQuery] string continuationToken = null, [FromQuery] int pageSize = 30)
        {
            var result = await _movieService.GetMoviesWithPaginationAsync(continuationToken, pageSize);
            return Ok(new
            {
                Movies = result.Items,
                ContinuationToken = result.ContinuationToken
            });
        }
    }
}
