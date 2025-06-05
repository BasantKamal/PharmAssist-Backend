namespace PharmAssist.DTOs
{
    public class AdminUserDTO
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime RegistrationDate { get; set; }
    }

    public class AdminUserDetailDTO : AdminUserDTO
    {
        public string PromptReason { get; set; }
        public string HasChronicConditions { get; set; }
        public string TakesMedicationsOrTreatments { get; set; }
        public string CurrentSymptoms { get; set; }
        public AddressDTO Address { get; set; }
        public IEnumerable<AdminOrderSummaryDTO> Orders { get; set; }
    }

    public class UpdateUserRolesDTO
    {
        public List<string> Roles { get; set; } = new List<string>();
    }
} 