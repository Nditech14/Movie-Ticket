using Application.Dto;
using Application.ResponseDto;
using Application.Service.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private readonly IProducerService _producerService;
        private readonly IMovieService _movieService;
        public ProducersController(IProducerService producerService, IMovieService movieService)
        {
            _producerService = producerService;
            _movieService = movieService;
        }

        // 1. Add a new producer
        [HttpPost("add-producer")]
        public async Task<ActionResult<ProducerResponseDto>> AddProducer([FromForm] ProducerDto producerDto)
        {
            var producer = await _producerService.AddProducerAsync(producerDto);
            return CreatedAtAction(nameof(GetProducerById), new { id = producer.Id }, producer);
        }



        // 3. Delete a producer by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProducer(string id)
        {
            var result = await _producerService.DeleteProducerAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // 4. Get all producers
        [HttpGet("Get-all-producer")]
        public async Task<ActionResult<IEnumerable<ProducerResponseDto>>> GetAllProducers()
        {
            var producers = await _producerService.GetAllProducersAsync();
            return Ok(producers);
        }

        // 5. Get a producer by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProducerResponseDto>> GetProducerById(string id)
        {
            var producer = await _producerService.GetProducerByIdAsync(id);
            return Ok(producer);
        }



        [HttpPatch("{id}/image")]
        public async Task<IActionResult> PatchProducerImage(string id, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Image file is required.");
            }

            try
            {
                // Update the actor's image
                var producerdto = new UpdateProducerDto
                {
                    ImageFile = imageFile
                };

                var updatedProducer = await _producerService.UpdateProducerAsync(id, producerdto);

                // Ensure the movie entities that reference this actor are also updated
                await _movieService.UpdateMoviesWithProducerImageAsync(updatedProducer.Id, updatedProducer.ImageUrl);

                return Ok(updatedProducer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
