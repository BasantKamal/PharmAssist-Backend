using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PharmAssist.Core.Services;
using System.Text;

namespace PharmAssist.APIs.Helpers
{
	public class CachedAttribute : Attribute , IAsyncActionFilter
	{
		private readonly int _expireTimeInSec;
		
		public CachedAttribute(int expireTimeInSec)
        {
			_expireTimeInSec = expireTimeInSec;
		}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var cacheService= context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
			var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
			var cachedResponse = await cacheService.GetCachedResponse(cacheKey);
			if (!string.IsNullOrEmpty(cachedResponse))
			{
				var contentResult = new ContentResult()
				{
					Content = cachedResponse,
					ContentType = "application/json",
					StatusCode = 200
				};
				context.Result = contentResult;
				return;
			}

			var executedEndpointContext = await next.Invoke(); 
			if(executedEndpointContext.Result is OkObjectResult result)
			{
				await cacheService.CacheReponseAsync(cacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSec));
			}
		}

		private string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			var keyBuilder=new StringBuilder();
			keyBuilder.Append(request.Path);  // api/products => ay controller msh lazem products
			foreach (var (key , value) in request.Query.OrderBy(x=>x.Key))
			{
				//sort=name , page index=1 , page size=5
				keyBuilder.Append($"|{key}-{value}");
				// api/products|sort=name|pageIndex=1|pageSize=5
			}
			return keyBuilder.ToString();
		}
	}
}
