﻿using GenAIUseCase1.Interfaces;

namespace GenAIUseCase1.Services {
	internal class HttpClientService : IHttpClientService {

		private readonly IHttpClientFactory _httpClientFactory;

		internal HttpClientService(IHttpClientFactory httpClientFactory) {
			_httpClientFactory = httpClientFactory;
		}

		public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken) {
			var httpClient = _httpClientFactory.CreateClient();

			var response = await httpClient.GetAsync(requestUri, cancellationToken);
			return response;
		}
	}
}