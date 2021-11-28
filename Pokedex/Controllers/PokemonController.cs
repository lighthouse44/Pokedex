using Microsoft.AspNetCore.Mvc;
using Pokedex.Models;
using Pokedex.Services;

namespace Pokedex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService, ILogger<PokemonController> logger)
        {
            _pokemonService = pokemonService;
            _logger = logger;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<IPokemon>> Get(string name)
        {
            var result = await _pokemonService.GetPokemonAsync(name);

            if (result.Success == false)
            {
                _logger.LogError($"Get pokemon {name} request failed status code {result.StatusCode}");

                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                else
                    return BadRequest();
            }

            _logger.LogInformation($"Get pokemon {name} request succeeded");
            return Ok(result.Result);
        }


        [HttpGet("translated/{name}")]
        public async Task<ActionResult<IPokemon>> GetTranslated(string name)
        {
            var result = await _pokemonService.GetTranslatedPokemonAsync(name);

            if (result.Success == false)
            {
                _logger.LogError($"Get translated pokemon {name} request failed status code {result.StatusCode}");
                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                else
                    return BadRequest();
            }

            _logger.LogInformation($"Get translated pokemon {name} request succeeded");
            return Ok(result.Result);
        }
    }
}
