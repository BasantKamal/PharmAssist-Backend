using PharmAssist.Errors;
using StackExchange.Redis;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace PharmAssist.MiddleWares
{
	public class ExceptionMiddleWare
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleWare> _logger;
		private readonly IHostEnvironment _env;

		public ExceptionMiddleWare(RequestDelegate Next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment env)
		{
			_next = Next;
			_logger = logger;
			_env = env;
		}

		//InvokeAsync(call function)
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				
				// Set appropriate status code
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				
				// Create response based on exception type
				ApiExceptionResponse response;
				
				if (ex is RedisConnectionException || ex is SocketException)
				{
					// Handle Redis connection issues
					_logger.LogError(ex, "Redis connection error");
					response = new ApiExceptionResponse(
						(int)HttpStatusCode.InternalServerError,
						"Cache service is currently unavailable. Please try again later.",
						_env.IsDevelopment() ? ex.StackTrace?.ToString() : null
					);
				}
				else
				{
					// Handle other exceptions
					response = _env.IsDevelopment()
						? new ApiExceptionResponse(
							(int)HttpStatusCode.InternalServerError,
							ex.Message,
							ex.StackTrace?.ToString()
						)
						: new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
				}
				
				var options = new JsonSerializerOptions()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				
				var jsonResponse = JsonSerializer.Serialize(response, options);
				await context.Response.WriteAsync(jsonResponse);
			}
		}
	}
}
