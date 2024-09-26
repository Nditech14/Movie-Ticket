using Core.Entities;
using Core.Enum;

namespace Application.ResponseDto
{
    public class MovieResponseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        public DateTime ReleaseDate { get; set; }
        public MovieStatus Status { get; set; }
        public List<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public List<Actor> Actors { get; set; }
        public List<Producer> Producers { get; set; }
        public Genre Genre { get; set; }
        public string? ImageUrl { get; set; }
    }
}
