using Newtonsoft.Json;

namespace Core.Entities
{
    public class Cart
    {

        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string userId { get; set; }  // Email of the user who owns the cart
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date when the cart was created
        public DateTime? LastUpdated { get; set; }  // Date when the cart was last updated
    }
}
