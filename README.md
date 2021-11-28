# Pokedex
This is a .NET 6 REST api built for the truelayer coding challenge

## API
This API has 2 endpoints

Endpoint 1:

GET /pokemon/{pokeman_name}

For example {base_url}/pokemon/mewtwo

This will return a response containing:
- Id: this represents the pokemon's id
- Name: this represents the pokemon's name
- Description: this represents the pokemon's description (if available in english)
- Habitat: this represents the pokemon's habitat (if available)
- IsLegendary: this represents the pokemon's legendary status

Endpoint 2:

This is similar to endpoint 1 with the difference of the description being translated.

If the pokemon is legendary or if the habitat is cave then a yoda translation is returned else a shakespeare translation is returned.

However, this translation depends on a third party API and if this fails the normal untranslated description is returned.

GET /pokemon/translated/{pokeman_name}

For example {base_url}/pokemon/translated/mewtwo

This will return a response containing:
- Id: this represents the pokemon's id
- Name: this represents the pokemon's name
- Description: this represents the pokemon's translated description (if available)
- Habitat: this represents the pokemon's habitat (if available)
- IsLegendary: this represents the pokemon's legendary status

## How to run
Step 0:

Requires .NET 6 to be installed, please see https://dotnet.microsoft.com/download/dotnet/6.0

Step 1:

Clone repo

Step 2:

In command prompt navigate to \Pokedex\Pokedex and run command "dotnet run"

This will start the API on ports 7171 (HTTPS) and 7272 (HTTP)

Step 3:

To interact with API within the browser via swagger navigate to https://localhost:7171/swagger/index.html

Here you will be able to plug in the name of the pokemon and view the result of the API via GUI

## Dependencies
Language:
- .NET 6
Third party APIs:
- PokeApi : https://pokeapi.co/
- Shakespeare translator: https://funtranslations.com/api/shakespeare
- Shakespeare translator: https://funtranslations.com/api/yoda

Nuget packages:
- Newtonsoft.Json Version=13.0.1
- Swashbuckle.AspNetCore Version=6.2.3
- Microsoft.Extensions.Logging.Abstractions Version=6.0.0
- Microsoft.NET.Test.Sdk" Version=16.11.0
- Moq Version=4.16.1
- NUnit Version=3.13.2
- NUnit3TestAdapter Version=4.0.0
- coverlet.collector Version=3.1.0

## 
TODO for prod api
- Client level unit tests, only did service level in interest of time
- Potentially add versioning to api /v1/..
