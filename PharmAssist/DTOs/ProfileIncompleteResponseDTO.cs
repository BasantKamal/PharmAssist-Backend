using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.DTOs
{
    public class ProfileIncompleteResponseDTO
    {
        public bool IsProfileComplete { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> MissingFields { get; set; } = new List<string>();
        public string ActionRequired { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
} 