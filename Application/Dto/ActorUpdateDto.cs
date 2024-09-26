using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class ActorUpdateDto
    {
        public IFormFile? ImageFile { get; set; }
    }
}
