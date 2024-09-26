using Core.Enum;
using Newtonsoft.Json;

namespace Core.Entities
{
    public class Movie
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<string> ActorIds { get; set; }
        public List<string> ProducerIds { get; set; }
        public List<string> CinemaIds { get; set; }
        public DateTime ReleaseDate { get; set; }
        public MovieStatus Status { get; set; }
        public List<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<Producer> Producers { get; set; } = new List<Producer>();
        public Genre Genre { get; set; }
        public string? ImageUrl { get; set; }
    }
}
