using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using PharmAssist.Repository.Data;

namespace PharmAssist.Repository
{
    public class DbBasketRepository : IBasketRepository
    {
        private readonly StoreContext _context;
        private readonly ILogger<DbBasketRepository> _logger;

        public DbBasketRepository(StoreContext context, ILogger<DbBasketRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            try
            {
                var basket = await _context.Baskets
                    .FirstOrDefaultAsync(b => b.Id == basketId);

                if (basket == null)
                    return false;

                _context.Baskets.Remove(basket);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting basket with ID: {BasketId}", basketId);
                throw;
            }
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            try
            {
                return await _context.Baskets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.Id == basketId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting basket with ID: {BasketId}", basketId);
                throw;
            }
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            try
            {
                foreach (var item in basket.Items)
                {
                    item.BasketId = basket.Id;
                }

                var existingBasket = await _context.Baskets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.Id == basket.Id);

                if (existingBasket == null)
                {
                    _context.Baskets.Add(basket);
                }
                else
                {
                    // Update existing basket
                    // Create a dictionary of existing items for easy lookup
                    var existingItems = existingBasket.Items.ToDictionary(i => i.ProductId);

                    foreach (var item in basket.Items)
                    {
                        if (existingItems.TryGetValue(item.ProductId, out var existingItem))
                        {
                            // Update existing item
                            existingItem.Quantity = item.Quantity;
                            existingItems.Remove(item.ProductId); // Remove from dictionary to track which items to delete
                        }
                        else
                        {
                            // Add new item
                            item.BasketId = existingBasket.Id;
                            existingBasket.Items.Add(item);
                        }
                    }

                    // Remove items that are no longer in the basket
                    foreach (var itemToDelete in existingItems.Values)
                    {
                        _context.BasketItems.Remove(itemToDelete);
                    }
                }

                await _context.SaveChangesAsync();
                return await GetBasketAsync(basket.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating basket with ID: {BasketId}", basket.Id);
                throw;
            }
        }
    }
} 