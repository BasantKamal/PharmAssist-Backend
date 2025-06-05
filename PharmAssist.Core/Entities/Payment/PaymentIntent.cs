namespace PharmAssist.Core.Entities.Payment
{
    public class PaymentIntent : BaseEntity
    {
        public string StripePaymentIntentId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public string BuyerEmail { get; set; }
        public string BasketId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public string ClientSecret { get; set; }
    }
} 