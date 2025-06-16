using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Entities
{
    public class MedicationRecommendation : BaseEntity
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public double SafetyScore { get; set; }
        public double EffectivenessScore { get; set; }
        public double ValueScore { get; set; }
        public double FinalScore { get; set; }
        public bool HasConflict { get; set; }
        public string ConflictDetails { get; set; } = string.Empty;
        public string RecommendationReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
} 