using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace PharmAssist.Controllers
{
	public class PredictController : APIBaseController
	{
		private readonly IHttpClientFactory _httpClient;
		private readonly string _aiServiceUrl;

		public PredictController(IHttpClientFactory httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_aiServiceUrl = configuration["AIService:BaseUrl"] ?? "http://127.0.0.1:5000";
		}

		[HttpPost]
		public async Task<IActionResult> GetPrediction([FromBody] PredictionRequestDto requestDto)
		{
			var client = _httpClient.CreateClient();
			client.Timeout = TimeSpan.FromMinutes(5);

			var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

			try
			{
				var resp = await client.PostAsync(_aiServiceUrl, content);
				if (!resp.IsSuccessStatusCode)
					return StatusCode((int)resp.StatusCode, "Error contacting AI service");

				var resultJson = await resp.Content.ReadAsStringAsync();
				Console.WriteLine("AI returned: " + resultJson);

				return Content(resultJson, "application/json", Encoding.UTF8);
			}
			catch (TaskCanceledException)
			{
				return StatusCode(504, "Request timed out waiting for AI service.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal error: {ex.Message}");
			}
		}
	}

	// DTO representing the input from your frontend to your backend /predict endpoint.
	public class PredictionRequestDto
	{
		public string CurrentSymptoms { get; set; }
		public string HasChronicConditions { get; set; }
		public string TakesMedicationsOrTreatments { get; set; }
	}
}