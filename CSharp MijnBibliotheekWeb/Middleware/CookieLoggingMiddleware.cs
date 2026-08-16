using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MijnBibliotheekWeb.Middleware;

/// Middleware die cookies logt en, indien aanwezig, cultuur instelt via cookie "_culture"
public class CookieLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CookieLoggingMiddleware> _logger;

    public CookieLoggingMiddleware(RequestDelegate next, ILogger<CookieLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log cookie namen en waarden (niet-gevoelige)
        foreach (var c in context.Request.Cookies)
        {
            _logger.LogDebug("Cookie: {Name}={Value}", c.Key, c.Value);
        }

        // Stel cultuur indien cookie aanwezig is
        if (context.Request.Cookies.TryGetValue("_culture", out var cul) && !string.IsNullOrWhiteSpace(cul))
        {
            try
            {
                var culture = new CultureInfo(cul);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            catch
            {
                // negeren bij ongeldige waarde
            }
        }

        await _next(context);
    }
}