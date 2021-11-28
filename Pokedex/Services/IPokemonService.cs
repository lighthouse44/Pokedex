using Pokedex.Models;
using Pokedex.Models.Results;

namespace Pokedex.Services
{
    public interface IPokemonService
    {
        Task<GenericResult<Pokemon>> GetPokemonAsync(string name);
        Task<GenericResult<Pokemon>> GetTranslatedPokemonAsync(string name);
    }
}
