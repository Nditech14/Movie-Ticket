using Core.Enum;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class TicketPurchase
    {
        [JsonProperty("id")]
        public string id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string MovieId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}

