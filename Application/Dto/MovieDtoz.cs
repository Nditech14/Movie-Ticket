using Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.Dto
{
    public class MovieDtoz
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1000.00.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Release date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "At least one producer is required.")]
        public List<ProducerDtoz> Producers { get; set; } = new List<ProducerDtoz>();

        [Required(ErrorMessage = "At least one actor is required.")]
        public List<ActorDtoz> Actors { get; set; } = new List<ActorDtoz>();

        [Required(ErrorMessage = "Movie status is required.")]
        public MovieStatus Status { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public Genre Genre { get; set; }
    }
}
