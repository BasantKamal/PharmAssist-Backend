
using PharmAssist.Core.Services;
using StackExchange.Redis;
using System.Text.Json;


namespace PharmAssist.Service
{
	public class ResponseCacheService : IResponseCacheService
	{
		private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer Redis)
        {
			_database=Redis.GetDatabase();
        }
		public async Task CacheReponseAsync(string CacheKey, object Response, TimeSpan ExpireTime)
		{
			if (Response is null) return;
			var options = new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
			var serializedResponse=JsonSerializer.Serialize(Response,options);
			await _database.StringSetAsync(CacheKey, serializedResponse, ExpireTime);
				
		}
		public async Task<string?> GetCachedResponse(string CacheKey)
		{
			var cachedResponse = await _database.StringGetAsync(CacheKey);
			if (cachedResponse.IsNullOrEmpty) return null;
			return cachedResponse;
		}
	}
}
