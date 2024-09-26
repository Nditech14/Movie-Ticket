using Newtonsoft.Json;

namespace Core.Entities
{
    public class TicketPayment
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TicketId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string CinemaId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } // e.g., Pending, Successful, Failed
        public string TransactionId { get; set; }
    }
}
