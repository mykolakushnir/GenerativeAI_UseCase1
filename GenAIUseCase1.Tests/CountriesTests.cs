using System.Collections.Generic;
using System.Linq;
using GenAIUseCase1.DTO;
using GenAIUseCase1.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenAIUseCase1.Tests {
	[TestClass]
	public class CountriesTests {

		#region Get Countries By Name Tests

		[TestMethod]
		public void GetCountriesByName_WhenMatchingCountryExists_ShouldReturnFilteredCountries() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "United States of America"}},
				new() {Name = new CountryName {Common = "Canada"}},
				new() {Name = new CountryName {Common = "United Kingdom"}},
			};
			var countryName = "United";

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByName(countries, countryName).ToList();

			// Assert
			Assert.AreEqual(2, filteredCountries.Count);
			Assert.IsTrue(filteredCountries.Any(c => c.Name.Common == "United States of America"));
			Assert.IsTrue(filteredCountries.Any(c => c.Name.Common == "United Kingdom"));
		}

		[TestMethod]
		public void GetCountriesByName_WhenNoMatchingCountryExists_ShouldReturnEmptyList() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "Germany"}},
				new() {Name = new CountryName {Common = "France"}},
			};
			var countryName = "Spain";

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByName(countries, countryName).ToList();

			// Assert
			Assert.AreEqual(0, filteredCountries.Count);
		}

		#endregion

		#region Get Countries By Population Tests

		[TestMethod]
		public void GetCountriesByPopulation_WhenMatchingCountriesExist_ShouldReturnFilteredCountries() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "CountryA"}, Population = 5_000_000},
				new() {Name = new CountryName {Common = "CountryB"}, Population = 10_000_000},
				new() {Name = new CountryName {Common = "CountryC"}, Population = 20_000_000},
			};
			var populationThreshold = 15; // This corresponds to 15 million people

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByPopulation(countries, populationThreshold).ToList();

			// Assert
			Assert.AreEqual(2, filteredCountries.Count);
			CollectionAssert.Contains(filteredCountries, countries[0]); // CountryA
			CollectionAssert.Contains(filteredCountries, countries[1]); // CountryB
		}

		[TestMethod]
		public void GetCountriesByPopulation_WhenNoMatchingCountriesExist_ShouldReturnEmptyList() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "CountryX"}, Population = 8_000_000},
				new() {Name = new CountryName {Common = "CountryY"}, Population = 12_000_000},
			};
			var populationThreshold = 5; // This corresponds to 5 million people

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByPopulation(countries, populationThreshold).ToList();

			// Assert
			Assert.AreEqual(0, filteredCountries.Count);
		}

		#endregion

		#region Sort Countries By Name Tests

		[TestMethod]
		public void SortCountriesByName_WhenSortOrderIsAscending_ShouldSortCountriesInAscendingOrder() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "Germany"}},
				new() {Name = new CountryName {Common = "France"}},
				new() {Name = new CountryName {Common = "Spain"}},
			};
			var sortOrder = "ascend";

			// Act
			var countryDataFilter = new CountryDataFilter();
			var sortedCountries = countryDataFilter.SortCountriesByName(countries, sortOrder).ToList();

			// Assert
			Assert.AreEqual(3, sortedCountries.Count);
			Assert.AreEqual("France", sortedCountries[0].Name.Common);
			Assert.AreEqual("Germany", sortedCountries[1].Name.Common);
			Assert.AreEqual("Spain", sortedCountries[2].Name.Common);
		}

		[TestMethod]
		public void SortCountriesByName_WhenSortOrderIsDescending_ShouldSortCountriesInDescendingOrder() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "Germany"}},
				new() {Name = new CountryName {Common = "France"}},
				new() {Name = new CountryName {Common = "Spain"}},
			};
			var sortOrder = "descend";

			// Act
			var countryDataFilter = new CountryDataFilter();
			var sortedCountries = countryDataFilter.SortCountriesByName(countries, sortOrder).ToList();

			// Assert
			Assert.AreEqual(3, sortedCountries.Count);
			Assert.AreEqual("Spain", sortedCountries[0].Name.Common);
			Assert.AreEqual("Germany", sortedCountries[1].Name.Common);
			Assert.AreEqual("France", sortedCountries[2].Name.Common);
		}

		[TestMethod]
		public void SortCountriesByName_WhenSortOrderIsInvalid_ShouldLeaveCountriesUnsorted() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "Germany"}},
				new() {Name = new CountryName {Common = "France"}},
				new() {Name = new CountryName {Common = "Spain"}},
			};
			var sortOrder = "invalidSortOrder";

			// Act
			var countryDataFilter = new CountryDataFilter();
			var sortedCountries = countryDataFilter.SortCountriesByName(countries, sortOrder).ToList();

			// Assert
			CollectionAssert.AreEqual(countries, sortedCountries);
		}

		#endregion

		#region Get Countries By Page Limit Tests

		[TestMethod]
		public void GetCountriesByPageLimit_WhenPageLimitIsGreaterThanCount_ShouldReturnAllCountries() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "CountryA"}},
				new() {Name = new CountryName {Common = "CountryB"}},
				new() {Name = new CountryName {Common = "CountryC"}},
			};
			var pageLimit = 5; // Page limit greater than the number of countries

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByPageLimit(countries, pageLimit).ToList();

			// Assert
			CollectionAssert.AreEqual(countries, filteredCountries);
		}

		[TestMethod]
		public void GetCountriesByPageLimit_WhenPageLimitIsLessThanCount_ShouldReturnLimitedNumberOfCountries() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "CountryA"}},
				new() {Name = new CountryName {Common = "CountryB"}},
				new() {Name = new CountryName {Common = "CountryC"}},
			};
			var pageLimit = 2; // Page limit less than the number of countries

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByPageLimit(countries, pageLimit).ToList();

			// Assert
			Assert.AreEqual(2, filteredCountries.Count);
			Assert.AreEqual("CountryA", filteredCountries[0].Name.Common);
			Assert.AreEqual("CountryB", filteredCountries[1].Name.Common);
		}

		[TestMethod]
		public void GetCountriesByPageLimit_WhenPageLimitIsZero_ShouldReturnEmptyList() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "CountryA"}},
				new() {Name = new CountryName {Common = "CountryB"}},
			};
			var pageLimit = 0; // Page limit is zero

			// Act
			var countryDataFilter = new CountryDataFilter();
			var filteredCountries = countryDataFilter.GetCountriesByPageLimit(countries, pageLimit).ToList();

			// Assert
			Assert.AreEqual(0, filteredCountries.Count);
		}

		#endregion
	}
}