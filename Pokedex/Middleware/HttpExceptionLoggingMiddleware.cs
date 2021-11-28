namespace Pokedex.Middleware
{
    public class HttpExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<HttpExceptionLoggingMiddleware> _logger;

        public HttpExceptionLoggingMiddleware(RequestDelegate requestDelegate, ILogger<HttpExceptionLoggingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
