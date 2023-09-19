using GenAIUseCase1.DTO;
using GenAIUseCase1.Helpers;
using GenAIUseCase1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GenAIUseCase1.Controllers {

	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase {

		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ICountryDataFilter _countryDataFilter;

		public CountriesController(IHttpClientFactory httpClientFactory) : this (httpClientFactory, new CountryDataFilter())
		{
			
		}

		internal CountriesController(IHttpClientFactory httpClientFactory, ICountryDataFilter countryDataFilter)
		{
			_httpClientFactory = httpClientFactory;
			_countryDataFilter = countryDataFilter;
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
						filteredCountries = _countryDataFilter.GetCountriesByName(filteredCountries, countryName).ToList();
					}

					// apply countries filtering by population
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = _countryDataFilter.GetCountriesByPopulation(filteredCountries, countryPopulation).ToList();
					}

					// apply page limitation
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = _countryDataFilter.GetCountriesByPageLimit(filteredCountries, limit).ToList();
					}

					// apply sorting
					if (!string.IsNullOrEmpty(countryName)) {
						filteredCountries = _countryDataFilter.SortCountriesByName(filteredCountries, sortType).ToList();
					}
				}
				
				return Ok(filteredCountries);
			}
			catch (Exception ex) {
				return StatusCode(500, ex.Message);
			}
		}

		
	}


}
