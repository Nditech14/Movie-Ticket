using Core.Entities;

namespace Application.ResponseDto
{
    public class CartResponseDto
    {
        public string Id { get; set; }

        public string UserId { get; set; }  // User identifier (used as partition key)
        public List<CartItem> CartItems { get; set; }  // List of items in the cart
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date when the cart was created
        public DateTime? LastUpdated { get; set; }
    }
}
