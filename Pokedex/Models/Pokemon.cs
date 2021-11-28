namespace Pokedex.Models
{
    public class Pokemon : IPokemon
    {
        public Pokemon(int id, string name, string description, string habitat, bool islegendary)
        {
            Id = id;
            Name = name;
            Description = description;
            Habitat = habitat;
            IsLegendary = islegendary;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; set; }
        public string Habitat { get; private set; }
        public bool IsLegendary { get; private set; }

    }
}