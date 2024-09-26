using Newtonsoft.Json;

namespace Core.Entities
{
    public class Cinema
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CinemaName { get; set; }
        public string MovieId { get; set; }

    }
}
