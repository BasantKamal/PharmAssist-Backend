
using Microsoft.AspNetCore.Hosting.Server;

namespace PharmAssist.Errors
{
	public class ApiResponse
	{	
		public int StatusCode { get; set; }
		public string? Message { get; set; }
        public ApiResponse(int statusCode,string? message=null)
        {
            StatusCode = statusCode;
            Message = message ?? getDefaultMessageForStatusCode(StatusCode);
        }

		private string? getDefaultMessageForStatusCode(int? statusCode)
		{
			//500 -> internal server error
			//400 -> bad request
			//401 -> unauthorized
			//404 -> not found
			return StatusCode switch
			{
				400 => "Bad Request",
				401 => "Unauthorized",
				404 => "Not Found",
				500 => "Internal Server Error",
				_ => null,
			};

		 }
	}
}
