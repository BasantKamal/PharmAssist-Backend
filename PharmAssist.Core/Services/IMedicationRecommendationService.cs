using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Services
{
    public interface IMedicationRecommendationService
    {
        Task<IReadOnlyList<MedicationRecommendation>> GenerateRecommendationsAsync(string userId, bool includeConflicted = false, int maxResults = 10);
        Task<MedicationRecommendation> AnalyzeProductSafetyAsync(Product product, string userId);
        Task<(int totalSafe, int totalConflicted, string summary)> GetSafetySummaryAsync(string userId);
        Task<Dictionary<string, object>> GetUserMedicalProfileAsync(string userId);
        bool IsUserProfileComplete(AppUser user);
        double AssessSafety(string ingredient, string conflicts, List<string> conditions);
        double CalculateEffectivenessScore(string ingredient);
        double CalculateValueScore(decimal price, double effectiveness);
        double CalculateFinalScore(double safety, double effectiveness, double value, bool hasConflict);
        Task<string> GenerateRecommendationReason(MedicationRecommendation recommendation, Product product);
        List<string> GenerateIntelligentInsights(List<MedicationRecommendation> recommendations, List<string> userConditions);
        Task<IReadOnlyList<MedicationRecommendation>> GetConflictingMedicationsAsync(string userId, int maxResults = 50);
    }
} 