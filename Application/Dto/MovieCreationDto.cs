using Core.Enum;

namespace Application.Dto
{
    public class MovieCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<ActorDtoz> Actors { get; set; } = new List<ActorDtoz>();
        public List<ProducerDtoz> Producers { get; set; } = new List<ProducerDtoz>();
        public List<CinemaDto> Cinemas { get; set; } = new List<CinemaDto>();
        public MovieStatus Status { get; set; }

        public Genre Genre { get; set; }
    }
}
