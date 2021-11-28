using Pokedex.Models;
using Pokedex.Models.Results;

namespace Pokedex.Services.Clients
{
    public interface IPokeApiClient
    {
        public Task<GenericResult<Pokemon>> GetPokemonAsync(string name);
    }
}
