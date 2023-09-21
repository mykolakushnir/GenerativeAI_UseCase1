# GenerativeAI Use Case #1
**C# project for GenerativeAI Use Case #1**

## Use Case Title
**Optimize data collected from an API to efficiently process and transform it into usable format for further representation.**

## Application description
Retrieve list of countries from [REST Countries][A1] API using name and population filters. 
Additionally collected data can be sorted or limited for pagination purpose.
Also country information is limited to represent only name and population.
Result data returned in JSON format for further representation.

### Features
- Filter by Country name
- Filter by Country population
- Sort results in ascending/descending order
- Limit results count which need to be returned to the client

### Sample of returned JSON Data
```sh
[
  {
    "name": {
      "common": "Christmas Island"
    },
    "population": 2072
  },
  {
    "name": {
      "common": "Liechtenstein"
    },
    "population": 38137
  },
  {
    "name": {
      "common": "Saint Helena, Ascension and Tristan da Cunha"
    },
    "population": 53192
  }
]
```

## Application endpoint summary
```sh
http|https://localhost:5000|7075/api/Countries/GetAllCountries?
countryName=[nameValue]&
countryPopulation=[populationValue]&
sortType=[sortValue]&
limit=[limitValue]
```
### Parameters usage
- **No parameters defined** - Return all data without any filters applied.

- **countryName** - Filter countries by name using specified [nameValue] of string type. Value can be partially defined. Filter is case insensitive.

- **countryPopulation** - Filter countries by population in millions using specified [populationValue] of int type.
E.g. provided value of 10 means get countries with population less then 10 millions.

- **sortType** - Sort countries in ascending/descending order using specified [sortValue] of string type. 
Allowed values - ascend, descend. [sortValue] is case insensitive. If this parameter not specified or incorrect value, sorting will remain intact.

- **limit** - Limit results using specified [limitValue] of int type. If this parameter not specified, all results will be returned.

## How to run the application locally
Open GenAIUseCase1.sln solution, then run application in debug mode. Swagger UI should be appear:
```sh
https://localhost:7075/swagger/index.html
```
Also you may call endpoint directly:
```sh
https://localhost:7075/api/Countries/GetAllCountries
```
Use parameters as described above.


## How to use the application endpoint
Launch application GenAIUseCase1.exe from bin\Release\net7.0\ folder of GenAIUseCase1 project.
Call endpoint in this way:
```sh
http://localhost:5000/api/Countries/GetAllCountries
```
Use parameters as described above.

## Examples
- **Get all countries info without filtering**
```sh
http://localhost:5000/api/Countries/GetAllCountries
```

- **Get all countries info where country name contains 'st'**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=st
```

- **Get all countries info where country population is less then 10 millions**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryPopulation=10
```

- **Get all countries info where results sorted in ascending order**
```sh
http://localhost:5000/api/Countries/GetAllCountries?sortType=ascend
```

- **Get all countries info where results are limited to 10 items**
```sh
http://localhost:5000/api/Countries/GetAllCountries?limit=10
```

- **Get all countries info where country name contains 'Us' and population is less then 50 millions**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=Us&countryPopulation=50
```

- **Get all countries info where country name contains 'Sa' and population is less then 1 million and sorted in descending order**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=Sa&countryPopulation=1&sortType=descend
```

- **Get all countries info where country name contains 'Sa' and population is less then 1 million, sorted in descending order and limit results to 3 items**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=Sa&countryPopulation=1&sortType=descend&limit=3
```

- **Get all countries info where country name contains 'San Marino', sorted in descending order and limit results to 10 items**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=San Marino&sortType=descend&limit=10
```

- **Get all countries info where country name contains 'United', sorted in ascending order and limit results to 300 items**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=United&sortType=ascend&limit=300
```

- **Get all countries info where country name contains 'Island' and sorted in ascending order**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryName=Island&sortType=ascend
```

- **Get all countries info where country population is less then 1 million and sorted in ascending order**
```sh
http://localhost:5000/api/Countries/GetAllCountries?countryPopulation=1&sortType=ascend
```






[//]: # (Reference links)
[A1]: <https://restcountries.com/v3.1/all>