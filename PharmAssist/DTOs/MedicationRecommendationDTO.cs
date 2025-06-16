using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.DTOs
{
    public class MedicationRecommendationDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string ProductPictureUrl { get; set; } = string.Empty;
        public string ActiveIngredient { get; set; } = string.Empty;
        public double SafetyScore { get; set; }
        public double EffectivenessScore { get; set; }
        public double ValueScore { get; set; }
        public double FinalScore { get; set; }
        public bool HasConflict { get; set; }
        public string ConflictDetails { get; set; } = string.Empty;
        public string RecommendationReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
} 