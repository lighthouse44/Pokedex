using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pokedex.Models;
using Pokedex.Models.Results;
using Pokedex.Services;
using Pokedex.Services.Clients;
using System.Net;
using System.Threading.Tasks;

namespace Pokedex.Tests
{
    public class PokemonServiceFixture
    {
        private Mock<IPokeApiClient> _pokeApiClient;
        private Mock<IFunTranslationsApiClient> _funTranslationApiClient;
        private Mock<ILogger<IPokemonService>> _logger;

        [SetUp]
        public void Setup()
        {
            _pokeApiClient = new Mock<IPokeApiClient>(MockBehavior.Strict);
            _funTranslationApiClient = new Mock<IFunTranslationsApiClient>(MockBehavior.Strict);
            _logger = new Mock<ILogger<IPokemonService>>();
        }

        [Test]
        public async Task GetPokemonAsync_WhenNameEmpty_ReturnBadRequest()
        {
            // Arrange
            string name = "";
            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = false, StatusCode = HttpStatusCode.BadRequest };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetPokemonAsync(name);

            // Assert
            Assert.False(result.Success);
            Assert.AreEqual(HttpStatusCode.BadRequest, pokemonResult.StatusCode);
        }

        [Test]
        public async Task GetPokemonAsync_WhenPokemonNotFound_ReturnNotFound()
        {
            // Arrange
            string name = "mewthree";
            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = false, StatusCode = HttpStatusCode.NotFound };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetPokemonAsync(name);

            // Assert
            Assert.False(result.Success);
            Assert.AreEqual(HttpStatusCode.NotFound, pokemonResult.StatusCode);
        }

        [Test]
        public async Task GetPokemonAsync_WhenPokemonFound_ReturnPokemon()
        {
            // Arrange
            int id = 1;
            string name = "mewtwo";
            string description = "A very unique pokemon";
            string habitat = "lab";
            bool isLegendary = true;

            Pokemon pokemon = new Pokemon(id, name, description, habitat, isLegendary);
                        
            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = true, StatusCode = HttpStatusCode.OK, Result = pokemon };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetPokemonAsync(name);

            // Assert

            Assert.True(result.Success);
            Assert.AreEqual(HttpStatusCode.OK, pokemonResult.StatusCode);
            Assert.AreEqual(pokemon, pokemonResult.Result);
        }

        [Test]
        public async Task GetTranslatedPokemonAsync_WhenNameEmpty_ReturnBadRequest()
        {
            // Arrange
            string name = "";
            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = false, StatusCode = HttpStatusCode.BadRequest };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetTranslatedPokemonAsync(name);

            // Assert
            Assert.False(result.Success);
            Assert.AreEqual(HttpStatusCode.BadRequest, pokemonResult.StatusCode);
        }

        [Test]
        public async Task GetTranslatedPokemonAsync_WhenPokemonNotFound_ReturnNotFound()
        {
            // Arrange
            string name = "mewthree";
            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = false, StatusCode = HttpStatusCode.NotFound };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetTranslatedPokemonAsync(name);

            // Assert
            Assert.False(result.Success);
            Assert.AreEqual(HttpStatusCode.NotFound, pokemonResult.StatusCode);
        }

        [Test]
        public async Task GetTranslatedPokemonAsync_WhenTranslationsFails_ReturnUnTranslatedDescription()
        {
            // Arrange
            int id = 1;
            string name = "geodude";
            string description = "Geo, dude";
            string habitat = "rock";
            bool isLegendary = false;

            Pokemon pokemon = new Pokemon(id, name, description, habitat, isLegendary);

            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = true, StatusCode = HttpStatusCode.OK, Result = pokemon };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            GenericResult<string> translation = new GenericResult<string> { Success = false, StatusCode = HttpStatusCode.TooManyRequests};

            _funTranslationApiClient.Setup(x => x.GetShakespeareTranslationAsync(description)).ReturnsAsync(translation);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetTranslatedPokemonAsync(name);

            // Assert
            Assert.True(result.Success);
            Assert.AreEqual(HttpStatusCode.OK, pokemonResult.StatusCode);
            Assert.AreEqual(name, pokemonResult.Result.Name);
            Assert.AreEqual(description, pokemonResult.Result.Description);
        }

        [Test]
        public async Task GetTranslatedPokemonAsync_WhenPokemonNotLegendaryOrCaveType_ReturnShakespeareTranslationPokemon()
        {
            // Arrange
            int id = 1;
            string name = "geodude";
            string description = "Geo, dude";
            string habitat = "rock";
            bool isLegendary = false;

            Pokemon pokemon = new Pokemon(id, name, description, habitat, isLegendary);

            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = true, StatusCode = HttpStatusCode.OK, Result = pokemon };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            string shakespeareText = "Geo art thou dude";
            GenericResult<string> translation = new GenericResult<string> { Success = true, StatusCode = HttpStatusCode.OK, Result = shakespeareText };

            _funTranslationApiClient.Setup(x => x.GetShakespeareTranslationAsync(description)).ReturnsAsync(translation);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetTranslatedPokemonAsync(name);

            // Assert
            Assert.True(result.Success);
            Assert.AreEqual(HttpStatusCode.OK, pokemonResult.StatusCode);
            Assert.AreEqual(name, pokemonResult.Result.Name);
            Assert.AreEqual(shakespeareText, pokemonResult.Result.Description);
        }

        [Test]
        public async Task GetTranslatedPokemonAsync_WhenPokemonIsLegendary_ReturnYodaTranslationPokemon()
        {
            // Arrange
            int id = 1;
            string name = "geodude";
            string description = "Geo, dude";
            string habitat = "rock";
            bool isLegendary = true;

            Pokemon pokemon = new Pokemon(id, name, description, habitat, isLegendary);

            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = true, StatusCode = HttpStatusCode.OK, Result = pokemon };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            string yodaText = "dude, Geo";
            GenericResult<string> translation = new GenericResult<string> { Success = true, StatusCode = HttpStatusCode.OK, Result = yodaText };

            _funTranslationApiClient.Setup(x => x.GetYogaTranslationAsync(description)).ReturnsAsync(translation);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetTranslatedPokemonAsync(name);

            // Assert
            Assert.True(result.Success);
            Assert.AreEqual(HttpStatusCode.OK, pokemonResult.StatusCode);
            Assert.AreEqual(name, pokemonResult.Result.Name);
            Assert.AreEqual(yodaText, pokemonResult.Result.Description);
        }

        [Test]
        public async Task GetTranslatedPokemonAsync_WhenPokemonHabitatIsCave_ReturnYodaTranslationPokemon()
        {
            // Arrange
            int id = 1;
            string name = "geodude";
            string description = "Geo, dude";
            string habitat = "cave";
            bool isLegendary = false;

            Pokemon pokemon = new Pokemon(id, name, description, habitat, isLegendary);

            GenericResult<Pokemon> result = new GenericResult<Pokemon> { Success = true, StatusCode = HttpStatusCode.OK, Result = pokemon };
            _pokeApiClient.Setup(x => x.GetPokemonAsync(name)).ReturnsAsync(result);

            string yodaText = "dude, Geo";
            GenericResult<string> translation = new GenericResult<string> { Success = true, StatusCode = HttpStatusCode.OK, Result = yodaText };

            _funTranslationApiClient.Setup(x => x.GetYogaTranslationAsync(description)).ReturnsAsync(translation);

            PokemonService service = CreatePokemonService();

            // Act
            var pokemonResult = await service.GetTranslatedPokemonAsync(name);

            // Assert
            Assert.True(result.Success);
            Assert.AreEqual(HttpStatusCode.OK, pokemonResult.StatusCode);
            Assert.AreEqual(name, pokemonResult.Result.Name);
            Assert.AreEqual(yodaText, pokemonResult.Result.Description);
        }

        private PokemonService CreatePokemonService()
        {
            return new PokemonService(_pokeApiClient.Object, _funTranslationApiClient.Object, _logger.Object);
        }
    }
}