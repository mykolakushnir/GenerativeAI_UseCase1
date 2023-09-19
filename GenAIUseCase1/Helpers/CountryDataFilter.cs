using GenAIUseCase1.DTO;
using GenAIUseCase1.Interfaces;

namespace GenAIUseCase1.Helpers {
	
	internal class CountryDataFilter : ICountryDataFilter {

		public IEnumerable<Country> GetCountriesByName(List<Country> countries, string countryName)
		{
			var nameFilter = countryName.ToLower();
			var filteredCountries = countries.Where(x => x.Name.Common.ToLower().Contains(nameFilter));
			return filteredCountries;
		}

		public IEnumerable<Country> GetCountriesByPopulation(List<Country> countries, int population)
		{
			int multiplier = 1000000;

			var populationFilter = population * multiplier;
			var filteredCountries = countries.Where(x => x.Population < populationFilter);
			return filteredCountries;
		}

		public IEnumerable<Country> SortCountriesByName(List<Country> countries, string sortOrder)
		{
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

		public IEnumerable<Country> GetCountriesByPageLimit(List<Country> countries, int pageLimit)
		{
			var filteredCountries = countries.Take(pageLimit);
			return filteredCountries;
		}
	}
}
