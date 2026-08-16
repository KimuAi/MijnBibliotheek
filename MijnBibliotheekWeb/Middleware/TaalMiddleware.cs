using System.Globalization;

namespace MijnBibliotheekWeb.Middleware;

// Eigen middleware om de taal te beheren
public class TaalMiddleware
{
    private readonly RequestDelegate _next;

    public TaalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check of er een taal-parameter in de URL zit (?culture=en)
        var cultureQuery = context.Request.Query["culture"];
        if (!string.IsNullOrWhiteSpace(cultureQuery))
        {
            var culture = new CultureInfo(cultureQuery);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            
            // Sla de voorkeur op in een cookie voor volgende requests
            context.Response.Cookies.Append(
                "MijnBibliotheek.Culture",
                cultureQuery,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }
        else
        {
            // Check cookie
            var cultureCookie = context.Request.Cookies["MijnBibliotheek.Culture"];
            if (!string.IsNullOrWhiteSpace(cultureCookie))
            {
                try 
                {
                    var culture = new CultureInfo(cultureCookie);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
                catch
                {
                    // Ongeldige culture in cookie, negeren
                }
            }
        }

        // Ga naar de volgende middleware in de pipeline
        await _next(context);
    }
}
