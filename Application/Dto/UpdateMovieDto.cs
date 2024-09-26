using Core.Enum;

namespace Application.Dto
{
    public class UpdateMovieDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public MovieStatus Status { get; set; }

        public Genre Genre { get; set; }
        public string? ImageUrl { get; set; }

    }
}