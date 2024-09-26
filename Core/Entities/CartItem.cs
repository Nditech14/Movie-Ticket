using Newtonsoft.Json;

namespace Core.Entities
{
    public class CartItem
    {

        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("userId")]
        public string userId { get; set; } // PartitionKey

        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;


    }
}
