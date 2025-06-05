namespace PharmAssist.DTOs
{
    public class SystemInfoDTO
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public string SystemVersion { get; set; }
        public DateTime LastBackupDate { get; set; }
        public string DatabaseStatus { get; set; }
    }
} 