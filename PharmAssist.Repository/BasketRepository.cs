using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Logging;


namespace PharmAssist.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDatabase _database;
		private readonly ILogger<BasketRepository> _logger;
		private readonly IConnectionMultiplexer _redis;

		public BasketRepository(IConnectionMultiplexer redis, ILogger<BasketRepository> logger) 
		{
			_redis = redis ?? throw new ArgumentNullException(nameof(redis), "Redis connection is required");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is required");
			
			if (!redis.IsConnected)
			{
				throw new InvalidOperationException("Redis connection is not available");
			}
			
			_database = redis.GetDatabase();
		}

		public async Task<bool> DeleteBasketAsync(string basketId)
		{
			try
			{
				EnsureRedisConnected();
				return await _database.KeyDeleteAsync(basketId);
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
				EnsureRedisConnected();
				var basket = await _database.StringGetAsync(basketId);
				
				if (basket.IsNull) return null;
				
				return JsonSerializer.Deserialize<CustomerBasket>(basket);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting basket with ID: {BasketId}", basketId);
				throw;
			}
		}

		public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket) //create or update
		{
			try
			{
				EnsureRedisConnected();
				var jsonBasket = JsonSerializer.Serialize(basket);
				var createdOrUpdated = await _database.StringSetAsync(basket.Id, jsonBasket, TimeSpan.FromDays(1)); //el time ely hto3odo(24 hrs)
				
				if (!createdOrUpdated) return null;
				
				return await GetBasketAsync(basket.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating basket with ID: {BasketId}", basket.Id);
				throw;
			}
		}

		private void EnsureRedisConnected()
		{
			if (_database == null || !_redis.IsConnected)
			{
				throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis connection is not available");
			}
		}
	}
}
