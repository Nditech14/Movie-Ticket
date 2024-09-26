using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class MovieImageUploadDto
    {
        public IFormFile? ImageFile { get; set; }
    }
}
