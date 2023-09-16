using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Linq;

namespace GenAIUseCase1.Controllers {

	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase {

		private readonly IHttpClientFactory _httpClientFactory;

		public CountriesController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet("GetAllCountries")]
		public async Task<IActionResult> GetAllCountries(string countryName, string countryPopulation, string sortType, int limit)
		{
			try {
				var apiUrl = "https://restcountries.com/v3.1/all";
				var httpClient = _httpClientFactory.CreateClient();

				var response = await httpClient.GetAsync(apiUrl);

				if (!response.IsSuccessStatusCode) {
					return StatusCode((int)response.StatusCode);
				}

				var contentStream = await response.Content.ReadAsStreamAsync();
				var countries = await JsonSerializer.DeserializeAsync<object>(contentStream);

				return Ok(countries);
			}
			catch (Exception ex) {
				return StatusCode(500, ex.Message);
			}
		}

	}


}
