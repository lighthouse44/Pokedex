using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pokedex.Models;
using Pokedex.Models.Results;
using System.Net;

namespace Pokedex.Services.Clients
{
    public class FunTranslationsApiClient : IFunTranslationsApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IFunTranslationsApiClient> _logger;
        private readonly string _clientBaseUrl;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
        };

        public FunTranslationsApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<IFunTranslationsApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _clientBaseUrl = configuration["ExternalRestApiUrls:FunTranslationsApiBase"];
            _logger = logger;
        }

        public async Task<GenericResult<string>> GetShakespeareTranslationAsync(string description)
        {
            return await GetTranslation($"{_clientBaseUrl}shakespeare.json?text={description}");
        }

        public async Task<GenericResult<string>> GetYogaTranslationAsync(string description)
        {
            return await GetTranslation($"{_clientBaseUrl}yoda.json?text={description}");
        }

        private async Task<GenericResult<string>> GetTranslation(string requestUrl)
        {
            HttpClient? client = _httpClientFactory.CreateClient();
            HttpResponseMessage? response = await client.GetAsync(requestUrl);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"FunTranslation request failed due to {response.StatusCode}");
                return new GenericResult<string>() { Success = false, StatusCode = response.StatusCode };
            }

            string json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(json))
            {
                _logger.LogError($"FunTranslation request returned empty translation");
                return new GenericResult<string>() { Success = false, StatusCode = HttpStatusCode.BadRequest };
            }

            FunTranslationsDTO dto = null;

            try
            {
                dto = JsonConvert.DeserializeObject<FunTranslationsDTO>(json, _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FunTranslation request failed trying to deserialize FunTranslationsDTO {ex.Message}");
                return new GenericResult<string>() { Success = false, StatusCode = HttpStatusCode.BadRequest };
            }

            if (string.IsNullOrEmpty(dto.Contents.Translated))
            {
                _logger.LogError($"FunTranslation request returned empty translation");
                return new GenericResult<string>() { Success = false, StatusCode = HttpStatusCode.BadRequest };
            }


            return new GenericResult<string>() { Success = true, Result = dto.Contents.Translated, StatusCode = HttpStatusCode.OK };
        }
    }
}