using PharmAssist.Errors;
using System.Net;
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
				_logger.LogError(ex,ex.Message);
				//production => log ex in db
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;  //same as 500

				//if (_env.IsDevelopment())
				//{
				//	var Response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString());
				//}
				//else
				//{
				//	var Response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
				//}

				var Response=_env.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
				var options = new JsonSerializerOptions()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				var jsonResponse=JsonSerializer.Serialize(Response,options);
				context.Response.WriteAsync(jsonResponse);

			}


		}
	}
}
