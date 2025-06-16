using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.DTOs
{
    public class MedicalProfileDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string PromptReason { get; set; } = string.Empty;
        public string HasChronicConditions { get; set; } = string.Empty;
        public string TakesMedicationsOrTreatments { get; set; } = string.Empty;
        public string CurrentSymptoms { get; set; } = string.Empty;
        public List<string> ParsedConditions { get; set; } = new List<string>();
    }
} 