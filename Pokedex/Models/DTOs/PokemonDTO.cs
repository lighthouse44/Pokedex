namespace Pokedex.Services.Clients
{
    public class PokemonDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<FlavorTextEntry> FlavorTextEntries { get; set; }

        public Habitat Habitat { get; set; }

        public bool IsLegendary { get; set; }
    }

    public class Habitat
    {
        public string Name { get; set; }
    }

    public class FlavorTextEntry
    {
        public string FlavorText { get; set; }
        public Language Language { get; set; }
    }

    public class Language
    {
        public string Name { get; set; }
    }
}
