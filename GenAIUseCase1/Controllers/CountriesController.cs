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

		/// <summary>
		/// Retrieve list of countries from "https://restcountries.com/v3.1/all" using name and population filters. Filters can be partially defined.
		/// Also result can be sorted or limited for pagination purpose.
		/// </summary>
		/// <param name="countryName">Filter by country name</param>
		/// <param name="countryPopulation">Filter by country population in millions. E.g. 10 means 10 millions. Returns counties which has less population then provided in filter.</param>
		/// <param name="sortType">Allows to sort results. Allowed values - ascend, descend</param>
		/// <param name="limit">Allows to limit results, useful for pagination</param>
		/// <returns>Return list of countries based on provided filters. If no filters/params specified returns all results.</returns>
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
					if (countryPopulation != 0) {
						filteredCountries = _countryDataFilter.GetCountriesByPopulation(filteredCountries, countryPopulation).ToList();
					}

					// apply sorting
					if (!string.IsNullOrEmpty(sortType)) {
						filteredCountries = _countryDataFilter.SortCountriesByName(filteredCountries, sortType).ToList();
					}

					// apply page limitation
					if (limit != 0) {
						filteredCountries = _countryDataFilter.GetCountriesByPageLimit(filteredCountries, limit).ToList();
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
