using System.Net;

namespace Pokedex.Models.Results
{
    public class GenericResult<TResult>
    {
        public bool Success { get; set; }
        public TResult Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
