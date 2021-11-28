namespace Pokedex.Models
{
    public class FunTranslationsDTO
    {
        public Success Success { get; set; }
        public Content Contents { get; set; }
    }

    public class Success
    {
        public int Total { get; set; }
    }

    public class Content
    {
        public string Translated { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }
    }
}
