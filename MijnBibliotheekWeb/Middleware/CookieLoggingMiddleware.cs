using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MijnBibliotheekWeb.Middleware;

/// Custom Middleware die cookies verwerkt en de taalcultuur instelt via de "_culture" cookie
public class CookieLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CookieLoggingMiddleware> _logger;

    // Constructor ontvangt de volgende middleware in de pijplijn en de logger
    public CookieLoggingMiddleware(RequestDelegate next, ILogger<CookieLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Wordt automatisch aangeroepen bij elk binnenkomend HTTP verzoek
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Log alle cookie namen voor debugging doeleinden
        foreach (var c in context.Request.Cookies)
        {
            _logger.LogDebug("Cookie ontvangen: {Name}={Value}", c.Key, c.Value);
        }

        // 2. Controleer of de gebruiker een voorkeurstaal heeft opgeslagen via de cookie "_culture"
        if (context.Request.Cookies.TryGetValue("_culture", out var cul) && !string.IsNullOrWhiteSpace(cul))
        {
            try
            {
                // Stel de gekozen cultuur in (bijv. "nl-BE" of "en-US") op het huidige uitvoerings-thread
                var culture = new CultureInfo(cul);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            catch
            {
                // Negeer eventuele ongeldige cultuurwaarden
            }
        }

        // 3. Geef het verzoek door aan de volgende middleware in de ASP.NET Core pijplijn
        await _next(context);
    }
}