using GenAIUseCase1.DTO;
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
		public async Task<IActionResult> GetAllCountries(string countryName = "", int countryPopulation = 0, string sortType = "", int limit = 0)
		{
			try {
				var apiUrl = "https://restcountries.com/v3.1/all";
				var httpClient = _httpClientFactory.CreateClient();

				var response = await httpClient.GetAsync(apiUrl);

				if (!response.IsSuccessStatusCode) {
					return StatusCode((int)response.StatusCode);
				}

				JsonSerializerOptions options = new JsonSerializerOptions {
					PropertyNameCaseInsensitive = true // Ignore case sensitivity when deserializing
				};

				var contentStream = await response.Content.ReadAsStreamAsync();
				var countries = await JsonSerializer.DeserializeAsync<List<Country>>(contentStream, options);
				
				var filteredCountries = new List<Country>();
				if (countries != null)
				{
					filteredCountries.AddRange(countries);

					// apply countries filtering by name
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = FilterCountriesByName(filteredCountries, countryName).ToList();
					}

					// apply countries filtering by population
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = FilterCountriesByPopulation(filteredCountries, countryPopulation).ToList();
					}

					// apply page limitation
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = GetCountriesByPageLimit(filteredCountries, limit).ToList();
					}

					// apply sorting
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = SortCountriesByName(filteredCountries, sortType).ToList();
					}
				}
				
				return Ok(filteredCountries);
			}
			catch (Exception ex) {
				return StatusCode(500, ex.Message);
			}
		}

		private IEnumerable<Country> FilterCountriesByName(List<Country> countries, string countryName) {
			var nameFilter = countryName.ToLower();
			var filteredCountries = countries.Where(x => x.Name.Common.ToLower().Contains(nameFilter));
			return filteredCountries;
		}

		private IEnumerable<Country> FilterCountriesByPopulation(List<Country> countries, int population) {
			int multiplier = 1000000;

			var populationFilter = population * multiplier;
			var filteredCountries = countries.Where(x => x.Population < populationFilter);
			return filteredCountries;
		}

		private IEnumerable<Country> SortCountriesByName(List<Country> countries, string sortOrder) {
			var ascSort = "ascend";
			var descSort = "descend";

			if (sortOrder.ToLower().Equals(ascSort)) {
				// ascend sort
				return countries.OrderBy(x => x.Name.Common);
			}

			if (sortOrder.ToLower().Equals(descSort)) {
				// descend sort
				return countries.OrderByDescending(x => x.Name.Common);
			}

			// leave countries unsorted
			return countries;
		}

		private IEnumerable<Country> GetCountriesByPageLimit(List<Country> countries, int pageLimit)
		{
			var filteredCountries = countries.Take(pageLimit);
			return filteredCountries;
		}
	}


}
