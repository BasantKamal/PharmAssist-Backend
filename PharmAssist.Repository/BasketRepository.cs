using PharmAssist.Core.Entities;
using PharmAssist.Core.Repositories;
using StackExchange.Redis;
using System.Text.Json;


namespace PharmAssist.Repository
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDatabase _database;

		public BasketRepository(IConnectionMultiplexer redis) 
		{
			_database = redis.GetDatabase();
		}
		public async Task<bool> DeleteBasketAsync(string basketId)
		{
			return await  _database.KeyDeleteAsync(basketId);
		}

		public async Task<CustomerBasket?> GetBasketAsync(string basketId)
		{
			var basket=await _database.StringGetAsync(basketId);
			
			return basket.IsNull ? null:JsonSerializer.Deserialize<CustomerBasket>(basket);

		}

		public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket) //create or update
		{
			var jsonBasket = JsonSerializer.Serialize(basket);
			var CreatedOrUpdated= await _database.StringSetAsync(basket.Id, jsonBasket,TimeSpan.FromDays(1)); //el time ely hto3odo(24 hrs)
			if (!CreatedOrUpdated) return null;
			
		    return await GetBasketAsync(basket.Id);
		}
	}
}
