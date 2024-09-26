using Core.Enum;

namespace Application.Dto
{
    public class MovieDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<ActorDto> Actors { get; set; }
        public List<ProducerDto> Producers { get; set; }
        public MovieStatus Status { get; set; }

        public Genre Genre { get; set; }


    }
}
