using PharmAssist.Errors;

namespace PharmAssist.Errors
{
	public class ApiValidationErrorResponse:ApiResponse
	{
		public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponse():base(400)
        {
             Errors=new List<string>();
        }
    }
}
