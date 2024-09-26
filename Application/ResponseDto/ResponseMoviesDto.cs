using Core.Enum;

namespace Application.ResponseDto
{
    public class ResponseMoviesDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        public DateTime ReleaseDate { get; set; }
        public MovieStatus Status { get; set; }

        public List<ActorResponseDto> Actors { get; set; }
        public List<ProducerResponseDto> Producers { get; set; }
        public Genre Genre { get; set; }
        public string? ImageUrl { get; set; }
    }
}
