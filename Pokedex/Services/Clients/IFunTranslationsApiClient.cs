using Pokedex.Models;
using Pokedex.Models.Results;

namespace Pokedex.Services.Clients
{
    public interface IFunTranslationsApiClient
    {
        public Task<GenericResult<string>> GetShakespeareTranslationAsync(string description);
        public Task<GenericResult<string>> GetYogaTranslationAsync(string nadescriptione);
    }
}
