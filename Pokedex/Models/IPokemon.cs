namespace Pokedex.Models
{
    public interface IPokemon
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; set; }
        public string Habitat { get; }
        public bool IsLegendary { get; }
    }
}
