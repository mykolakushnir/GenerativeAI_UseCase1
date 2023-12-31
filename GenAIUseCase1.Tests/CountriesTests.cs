﻿using System.Collections.Generic;
using System.Linq;
using GenAIUseCase1.Controllers;
using System.Net.Http;
using GenAIUseCase1.DTO;
using GenAIUseCase1.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using GenAIUseCase1.Interfaces;

namespace GenAIUseCase1.Tests {

	[TestClass]
	public class CountriesTests {

		private CountriesController _controller;
		private Mock<IHttpClientFactory> _httpClientFactoryMock;
		private Mock<IHttpClientService> _httpClientServiceMock;

		[TestInitialize]
		public void Setup()
		{
			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_httpClientServiceMock = new Mock<IHttpClientService>();
			ICountryDataFilter dataFilter = new CountryDataFilter();

			_controller = new CountriesController(_httpClientFactoryMock.Object, _httpClientServiceMock.Object, dataFilter);
		}

		#region Countries Controller Tests

		[TestMethod]
		public async Task GetAllCountries_SuccessfulResponse_ReturnsFilteredCountries() {
			// Arrange
			var countries = new List<Country>
			{
				new() {Name = new CountryName {Common = "CountryA"}, Population = 10000000},
				new() {Name = new CountryName {Common = "CountryB"}, Population = 20000000},
				new() {Name = new CountryName {Common = "CountryC"}, Population = 30000000},
			};

			_httpClientServiceMock
				.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(JsonSerializer.Serialize(countries)),
				});

			// Act
			var response = await _controller.GetAllCountries("Country", 1500000, "ascend", 2) as OkObjectResult;
			var result = response.Value as List<Country>;
			
			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual("CountryA", result[0].Name.Common);
			Assert.AreEqual("CountryB", result[1].Name.Common);
		}

		[TestMethod]
		public async Task GetAllCountries_UnsuccessfulResponse_ReturnsStatusCode() {
			// Arrange
			_httpClientServiceMock
				.Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.InternalServerError,
					Content = new StringContent("Internal Server Error"),
				});
			
			// Act
			var response = await _controller.GetAllCountries() as StatusCodeResult;

			// Assert
			Assert.IsNotNull(response);
			Assert.AreEqual(500, response.StatusCode);
		}

		#endregion

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
				new() {Name = new CountryName {Common = "CountryA"}, Population = 5000000},
				new() {Name = new CountryName {Common = "CountryB"}, Population = 10000000},
				new() {Name = new CountryName {Common = "CountryC"}, Population = 20000000},
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
				new() {Name = new CountryName {Common = "CountryX"}, Population = 8000000},
				new() {Name = new CountryName {Common = "CountryY"}, Population = 12000000},
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