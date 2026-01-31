using System.Net;

namespace MijnBibliotheekMAUI.Models;
// voor het bijhouden van de sessie-informatie van de API-gebruiker
public class ApiSession
{
    public CookieContainer Cookies { get; } = new();

    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public List<string> Roles { get; set; } = new();

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(UserName);
    public bool IsAdmin => Roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));

    public void Clear()
    {
        UserName = null;
        FullName = null;
        Roles = new();
  
    }

    public static string GetBaseUrl()
    {
#if ANDROID
        return "http://10.0.2.2:5203/";
#elif WINDOWS
        return "http://localhost:5203/";
#else
        return "http://localhost:5203/";
#endif
    }
}
