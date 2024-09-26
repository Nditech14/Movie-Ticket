using System.ComponentModel.DataAnnotations;

namespace Application.Dto
{
    public class AddToCartDto
    {
        [Required]
        public string MovieId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }


    }
}
