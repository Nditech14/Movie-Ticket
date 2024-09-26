namespace Core.Entities
{
    public class PaystackData
    {
        public string authorization_url { get; set; }
        public string reference { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
        public string payerEmail { get; set; }
        public string transactionId { get; set; }
    }
}
