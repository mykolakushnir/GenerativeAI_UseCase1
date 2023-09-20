using GenAIUseCase1.DTO;
using GenAIUseCase1.Helpers;
using GenAIUseCase1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using GenAIUseCase1.Services;

namespace GenAIUseCase1.Controllers {

	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase {

		private readonly ICountryDataFilter _countryDataFilter;
		private readonly IHttpClientService _httpClientService;

		public CountriesController(IHttpClientFactory httpClientFactory) : this (httpClientFactory, null, new CountryDataFilter())
		{
		}

		internal CountriesController(IHttpClientFactory httpClientFactory, IHttpClientService httpClientService, ICountryDataFilter countryDataFilter) {
			_httpClientService = httpClientService ?? new HttpClientService(httpClientFactory);
			_countryDataFilter = countryDataFilter;
		}

		[HttpGet("GetAllCountries")]
		public async Task<IActionResult> GetAllCountries(string countryName = "", int countryPopulation = 0, string sortType = "", int limit = 0)
		{
			try {
				var apiUrl = "https://restcountries.com/v3.1/all";

				var cancellationSource = new CancellationTokenSource();
				var cancellationToken = cancellationSource.Token;

				var response = await _httpClientService.GetAsync(apiUrl, cancellationToken);

				if (!response.IsSuccessStatusCode) {
					return StatusCode((int)response.StatusCode);
				}

				JsonSerializerOptions options = new JsonSerializerOptions {
					PropertyNameCaseInsensitive = true // Ignore case sensitivity when deserializing
				};

				var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
				var countries = await JsonSerializer.DeserializeAsync<List<Country>>(contentStream, options, cancellationToken);
				
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
				Console.WriteLine($"Error happen during request GetAllCountries - {ex.Message}");
				return StatusCode(500, ex.Message);
			}
		}

	}


}
