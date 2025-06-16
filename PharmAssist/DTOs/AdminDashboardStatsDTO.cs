namespace PharmAssist.DTOs
{
    public class AdminDashboardStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int OutForDeliveryOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<AdminOrderSummaryDTO> RecentOrders { get; set; }
    }
} 