using GenAIUseCase1.DTO;

namespace GenAIUseCase1.Interfaces {
	
	internal interface ICountryDataFilter {

		IEnumerable<Country> GetCountriesByName(List<Country> countries, string countryName);

		IEnumerable<Country> GetCountriesByPopulation(List<Country> countries, int population);

		IEnumerable<Country> SortCountriesByName(List<Country> countries, string sortOrder);

		IEnumerable<Country> GetCountriesByPageLimit(List<Country> countries, int pageLimit);
	}
}
