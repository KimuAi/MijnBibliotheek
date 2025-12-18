using System.Net;

namespace MijnBibliotheekMAUI.Models;

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
        return "https://10.0.2.2:7234/";
#elif WINDOWS
        return "https://localhost:7234/";
#else
        return "https://localhost:7234/";
#endif
    }
}
