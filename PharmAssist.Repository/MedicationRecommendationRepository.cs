using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Repository
{
    public class MedicationRecommendationRepository : GenericRepository<MedicationRecommendation>, IMedicationRecommendationRepository
    {
        public MedicationRecommendationRepository(StoreContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<MedicationRecommendation>> GetRecommendationsByUserIdAsync(string userId, bool includeConflicted = false, int maxResults = 10)
        {
            var query = _context.MedicationRecommendations
                .Include(r => r.Product)
                .Where(r => r.UserId == userId && r.IsActive);

            if (!includeConflicted)
            {
                query = query.Where(r => !r.HasConflict);
            }

            return await query
                .OrderByDescending(r => r.FinalScore)
                .Take(maxResults)
                .ToListAsync();
        }

        public async Task<MedicationRecommendation> GetRecommendationByUserAndProductAsync(string userId, int productId)
        {
            return await _context.MedicationRecommendations
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId && r.IsActive);
        }

        public async Task<IReadOnlyList<MedicationRecommendation>> GetActiveRecommendationsAsync(string userId)
        {
            return await _context.MedicationRecommendations
                .Include(r => r.Product)
                .Where(r => r.UserId == userId && r.IsActive)
                .OrderByDescending(r => r.FinalScore)
                .ToListAsync();
        }

        public async Task<int> GetSafeRecommendationsCountAsync(string userId)
        {
            return await _context.MedicationRecommendations
                .CountAsync(r => r.UserId == userId && r.IsActive && !r.HasConflict);
        }

        public async Task<int> GetConflictedRecommendationsCountAsync(string userId)
        {
            return await _context.MedicationRecommendations
                .CountAsync(r => r.UserId == userId && r.IsActive && r.HasConflict);
        }

        public async Task DeactivateOldRecommendationsAsync(string userId)
        {
            var oldRecommendations = await _context.MedicationRecommendations
                .Where(r => r.UserId == userId && r.IsActive)
                .ToListAsync();

            foreach (var recommendation in oldRecommendations)
            {
                recommendation.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
} 