using Pokedex.Middleware;
using Pokedex.Services;
using Pokedex.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IPokeApiClient, PokeApiClient>();
builder.Services.AddSingleton<IFunTranslationsApiClient, FunTranslationsApiClient>();
builder.Services.AddSingleton<IPokemonService, PokemonService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<HttpExceptionLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();