using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/actors")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;
        private readonly IMapper _mapper;
        private readonly IMovieService _movieService;

        public ActorController(IActorService actorService, IMapper mapper, IMovieService movieService)
        {
            _actorService = actorService;
            _mapper = mapper;
            _movieService = movieService;
        }

        [HttpPost("add-actor")]
        public async Task<IActionResult> AddActor([FromForm] ActorDto actorDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => new Error("ModelValidationError", e.ErrorMessage));
                return BadRequest(ResponseDto<ActorResponseDto>.Failure(errors));
            }

            var result = await _actorService.AddActorAsync(actorDto);
            return Ok(result);
        }

        [HttpPatch("{id}/image")]
        public async Task<IActionResult> PatchActorImage(string id, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Image file is required.");

            try
            {
                var actorDto = new ActorUpdateDto
                {
                    ImageFile = imageFile
                };

                var updatedActor = await _actorService.UpdateActorAsync(id, actorDto);
                await _movieService.UpdateMoviesWithActorImageAsync(updatedActor.Id, updatedActor.ImageFile);

                return Ok(updatedActor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete-actor/{id}")]
        public async Task<IActionResult> DeleteActor(string id)
        {
            var result = await _actorService.DeleteActorAsync(id);
            if (result)
                return Ok(new { message = $"Actor with ID '{id}' deleted successfully." });

            return NotFound(new { message = $"Actor with ID '{id}' not found." });
        }

        [HttpGet("all-actors")]
        public async Task<IActionResult> GetAllActors()
        {
            var result = await _actorService.GetAllActorsAsync();
            var domainResult = _mapper.Map<List<ActorResponseDto>>(result);
            return Ok(domainResult);
        }

        [HttpGet("get-actor/{id}")]
        public async Task<IActionResult> GetActorById(string id)
        {
            var result = await _actorService.GetActorByIdAsync(id);
            if (result != null)
                return Ok(result);

            return NotFound(new { message = $"Actor with ID '{id}' not found." });
        }
    }
}
