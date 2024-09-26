using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class UpdateProducerDto
    {
        public IFormFile? ImageFile { get; set; }
    }
}
