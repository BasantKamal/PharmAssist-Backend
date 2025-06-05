namespace PharmAssist.DTOs
{
    public class PaymentIntentDTO
    {
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
    }
} 