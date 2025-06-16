using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.DTOs
{
    public class SafetySummaryDTO
    {
        public int TotalSafeRecommendations { get; set; }
        public int TotalConflictedItems { get; set; }
        public string Summary { get; set; } = string.Empty;
        public MedicationRecommendationDTO? TopRecommendation { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string AIPersonalizedMessage { get; set; } = string.Empty;
        public string OverallRiskAssessment { get; set; } = string.Empty;
        public List<string> SafetyHighlights { get; set; } = new List<string>();
        public string RecommendedAction { get; set; } = string.Empty;
    }
} 