using PharmAssist.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Repositories
{
    public interface IMedicationRecommendationRepository : IGenericRepository<MedicationRecommendation>
    {
        Task<IReadOnlyList<MedicationRecommendation>> GetRecommendationsByUserIdAsync(string userId, bool includeConflicted = false, int maxResults = 10);
        Task<MedicationRecommendation> GetRecommendationByUserAndProductAsync(string userId, int productId);
        Task<IReadOnlyList<MedicationRecommendation>> GetActiveRecommendationsAsync(string userId);
        Task<int> GetSafeRecommendationsCountAsync(string userId);
        Task<int> GetConflictedRecommendationsCountAsync(string userId);
        Task DeactivateOldRecommendationsAsync(string userId);
    }
} 