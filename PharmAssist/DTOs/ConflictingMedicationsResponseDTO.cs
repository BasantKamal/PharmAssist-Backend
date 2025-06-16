using System;
using System.Collections.Generic;

namespace PharmAssist.DTOs
{
    public class ConflictingMedicationsResponseDTO
    {
        public List<MedicationRecommendationDTO> ConflictingMedications { get; set; }
        public int TotalConflictingItems { get; set; }
        public string Summary { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string AIPersonalizedMessage { get; set; }
        public List<string> ConflictWarnings { get; set; }
        public List<string> SafetyAdvice { get; set; }
    }
} 