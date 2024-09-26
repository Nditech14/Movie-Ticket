using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class ActorDto
    {
        public string FullName { get; set; }
        public string Biography { get; set; }
        public IFormFile? ImageFile { get; set; }


    }
}
