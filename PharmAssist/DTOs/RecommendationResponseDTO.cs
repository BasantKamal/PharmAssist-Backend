using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.DTOs
{
    public class RecommendationResponseDTO
    {
        public IReadOnlyList<MedicationRecommendationDTO> Recommendations { get; set; }
        public string Summary { get; set; } = string.Empty;
        public int TotalSafeRecommendations { get; set; }
        public int TotalConflictedItems { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string AIPersonalizedMessage { get; set; } = string.Empty;
        public string ConfidenceLevel { get; set; } = "High";
        public List<string> KeyInsights { get; set; } = new List<string>();
        public string NextStepsAdvice { get; set; } = string.Empty;
    }
} 