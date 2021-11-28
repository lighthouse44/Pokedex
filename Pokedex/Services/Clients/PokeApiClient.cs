using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pokedex.Models;
using Pokedex.Models.Results;
using System.Net;
using System.Text.RegularExpressions;

namespace Pokedex.Services.Clients
{
    public class PokeApiClient : IPokeApiClient
    {
        private const string English = "en";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IPokeApiClient> _logger;
        
        private readonly string _clientBaseUrl;

        private readonly Dictionary<string, GenericResult<Pokemon>> _pokemonResultCache = new();

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
        };

        public PokeApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<IPokeApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _clientBaseUrl = configuration["ExternalRestApiUrls:PokeApiBase"];
            _logger = logger;
        }

        public async Task<GenericResult<Pokemon>> GetPokemonAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogError($"Failed to request pokemon due to empty name");
                return new GenericResult<Pokemon>() { Success = false, StatusCode = HttpStatusCode.BadRequest };
            }

            if (_pokemonResultCache.ContainsKey(name))
            {
                _logger.LogInformation($"Retreived cached pokemon {name}");
                return _pokemonResultCache[name];
            }

            HttpClient? client = _httpClientFactory.CreateClient();
            HttpResponseMessage? response = await client.GetAsync($"{_clientBaseUrl}/v2/pokemon-species/{name}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"PokeApi request failed due to {response.StatusCode}");
                return new GenericResult<Pokemon> { Success = false, StatusCode = response.StatusCode };
            }

            string json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(json))
            {
                _logger.LogError($"PokeApi request failed due to empty content");
                return new GenericResult<Pokemon> { Success = false, StatusCode = HttpStatusCode.BadRequest };
            }

            PokemonDTO dto = null;
            try
            {
                dto = JsonConvert.DeserializeObject<PokemonDTO>(json, _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PokeApi request failed trying to deserialize PokemonDTO {ex.Message}");
            }

            if(dto == null || dto.Name.ToLower() != name.ToLower())
            {
                if(dto == null)
                    _logger.LogError($"PokeApi request failed trying to deserialize PokemonDTO");
                else
                    _logger.LogError($"PokeApi request failed due to name mismatch between returned {dto.Name.ToLower()} and requested {name}");

                return new GenericResult<Pokemon> { Success = false, StatusCode = HttpStatusCode.BadRequest};
            }

            GenericResult<Pokemon> result = new GenericResult<Pokemon>() { Success = true, Result = ConvertToPokemon(dto), StatusCode = HttpStatusCode.OK };

            _pokemonResultCache[name] = result;

            return result;
        }

        private Pokemon ConvertToPokemon(PokemonDTO dto)
        {
            return new Pokemon(
                id: dto.Id,
                name: dto.Name,
                description: GetValidFlavourText(dto.FlavorTextEntries),
                habitat: dto.Habitat?.Name,
                islegendary: dto.IsLegendary
                );
        }

        private string GetValidFlavourText(IList<FlavorTextEntry> flavorTextEntries)
        {
            foreach(var entry in flavorTextEntries)
            {
                if(entry.FlavorText != null && entry.Language.Name == English)
                    return Regex.Replace(entry.FlavorText, @"\r|\f|\n|\t|\v", " ");
            }

            _logger.LogWarning($"No valid flavourtext found, defaulting description to empty");
            return "";
        }
    }
}