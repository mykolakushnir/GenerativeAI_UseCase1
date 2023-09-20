namespace GenAIUseCase1.Interfaces {
	internal interface IHttpClientService {
		Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken);
	}
}
