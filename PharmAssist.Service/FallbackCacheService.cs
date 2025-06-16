using PharmAssist.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

namespace PharmAssist.Service
{
    public class FallbackCacheService : IResponseCacheService
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        
        private class CacheItem
        {
            public string Value { get; set; }
            public DateTime Expiry { get; set; }
        }

        public async Task CacheReponseAsync(string CacheKey, object Response, TimeSpan ExpireTime)
        {
            if (Response is null) return;
            
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var serializedResponse = JsonSerializer.Serialize(Response, options);
            
            _cache[CacheKey] = new CacheItem 
            { 
                Value = serializedResponse, 
                Expiry = DateTime.UtcNow.Add(ExpireTime) 
            };
            
            await Task.CompletedTask;
        }

        public async Task<string?> GetCachedResponse(string CacheKey)
        {
            if (_cache.TryGetValue(CacheKey, out var cacheItem))
            {
                if (DateTime.UtcNow < cacheItem.Expiry)
                {
                    return cacheItem.Value;
                }
                
                // Remove expired item
                _cache.TryRemove(CacheKey, out _);
            }
            
            return await Task.FromResult<string>(null);
        }
    }
} 