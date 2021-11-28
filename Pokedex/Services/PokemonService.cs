using Pokedex.Models;
using Pokedex.Models.Results;
using Pokedex.Services.Clients;

namespace Pokedex.Services
{
    public class PokemonService : IPokemonService
    {
        private const string Cave = "cave";
        private readonly IPokeApiClient _pokeApiClient;
        private readonly IFunTranslationsApiClient _funTranslationsApiClient;
        private readonly ILogger<IPokemonService> _logger;
        
        public PokemonService(IPokeApiClient pokeApiClient, IFunTranslationsApiClient funTranslationsApiClient, ILogger<IPokemonService> logger)
        {
            _pokeApiClient = pokeApiClient;
            _funTranslationsApiClient = funTranslationsApiClient;
            _logger = logger;
        }

        public async Task<GenericResult<Pokemon>> GetPokemonAsync(string name)
        {
            var result = await _pokeApiClient.GetPokemonAsync(name.ToLower());
            return result;
        }

        public async Task<GenericResult<Pokemon>> GetTranslatedPokemonAsync(string name)
        {
            var pokemonResult = await GetPokemonAsync(name);
            if(pokemonResult == null || pokemonResult.Success == false)
                return pokemonResult;

            GenericResult<string>? translationResult;

            if (pokemonResult.Result.Habitat == Cave || pokemonResult.Result.IsLegendary)
            {
                _logger.LogInformation($"Requesting yoda translation for {name}");
                translationResult = await _funTranslationsApiClient.GetYogaTranslationAsync(pokemonResult.Result.Description);
            }
            else
            {
                _logger.LogInformation($"Requesting shakespeare translation for {name}");
                translationResult = await _funTranslationsApiClient.GetShakespeareTranslationAsync(pokemonResult.Result.Description);
            }

            if (translationResult != null && translationResult.Success == true)
            {                
                pokemonResult.Result.Description = translationResult.Result;
            }
            else
            {
                _logger.LogWarning($"Translation failed for {name} defaulting to untranslated description");
            }

            return pokemonResult;
        }
    }
}
