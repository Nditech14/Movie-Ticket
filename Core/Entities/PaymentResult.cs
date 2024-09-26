namespace Core.Entities
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Refernce { get; set; }
        public string RedirectUrl { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string PayerEmail { get; set; }
        public decimal AmountPaid { get; set; }
        public string Currency { get; set; }


        public List<MovieInfo> PurchasedMovies { get; set; }
    }
}
