namespace Core.Entities
{
    public class PaystackResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PaystackData data { get; set; }
    }
}
