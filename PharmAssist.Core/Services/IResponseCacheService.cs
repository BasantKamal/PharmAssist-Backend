

namespace PharmAssist.Core.Services
{
	public interface IResponseCacheService
	{
		//Cache data

		Task CacheReponseAsync(string CacheKey, object Response, TimeSpan ExpireTime);

		//Get cached data (key,value)
		Task<string?> GetCachedResponse(string CacheKey);
	}
}
