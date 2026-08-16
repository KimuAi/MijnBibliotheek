using System.Net;
using System.Net.Http.Json;
using MijnBibliotheekMAUI.Dtos;

namespace MijnBibliotheekMAUI.Services;

// Service klasse die via HTTP (REST API) communiceert met de webserver, met automatische terugval op de lokale SQLite opslag
public class BibliotheekApiService
{
    private readonly HttpClient _http;
    private readonly LocalDbService _localDb;

    // Constructor via Dependency Injection
    public BibliotheekApiService(HttpClient http, LocalDbService localDb)
    {
        _http = http;
        _localDb = localDb;
    }

    // Haalt boeken op via de API, slaat ze op in SQLite. Bij netwerkfout worden ze uit SQLite geladen!
    public async Task<List<BoekDto>> GetBoekenAsync()
    {
        try
        {
            // Probeer gegevens via HTTP GET op te halen van de web API (/api/boekenapi)
            var remote = await GetAsync<List<BoekDto>>("api/boekenapi") ?? new();
            if (remote.Any())
            {
                // Als er boeken zijn opgehaald, sla deze direct lokaal op in SQLite voor offline gebruik
                await _localDb.SaveBoekenAsync(remote);
            }
            return remote;
        }
        catch (Exception ex)
        {
            // Bij een netwerkfout of als de server offline is: lees de eerder opgeslagen boeken uit het lokale SQLite databankje!
            System.Diagnostics.Debug.WriteLine($"API-fout bij ophalen boeken, laden uit offline SQLite: {ex.Message}");
            return await _localDb.GetBoekenAsync();
        }
    }

    // Haalt categorieën op via API of lokaal uit SQLite
    public async Task<List<CategorieDto>> GetCategorieenAsync()
    {
        try
        {
            var remote = await GetAsync<List<CategorieDto>>("api/categorieenapi") ?? new();
            if (remote.Any())
            {
                await _localDb.SaveCategorieenAsync(remote);
            }
            return remote;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API-fout bij ophalen categorieën, laden uit offline SQLite: {ex.Message}");
            return await _localDb.GetCategorieenAsync();
        }
    }

    // Haalt de eigen uitleningen op van de ingelogde gebruiker
    public async Task<List<UitleningDto>> GetMijnUitleningenAsync()
    {
        var bases = new[] { "api/uitleningenapi", "api/uitleningen" };
        var tails = new[] { "mijn", "mine", "me", "mijnuitleningen", "mijn-uitleningen", "user" };

        foreach (var b in bases)
        {
            foreach (var t in tails)
            {
                var url = $"{b}/{t}";
                try
                {
                    var (ok, data, code) = await TryGetAsync<List<UitleningDto>>(url);
                    if (ok && data != null)
                    {
                        // Sla de uitleningen lokaal op
                        await _localDb.SaveUitleningenAsync(data);
                        return data;
                    }
                }
                catch
                {
                    // Probeer de volgende URL variant
                }
            }
        }

        // Als geen enkele API-route bereikbaar is (offline): geef de lokaal opgeslagen uitleningen uit SQLite terug
        return await _localDb.GetUitleningenAsync();
    }

    // Stuurt een verzoek naar de API om een boek uit te lenen
    public async Task<bool> LeenBoekAsync(int boekId)
    {
        var candidates = new[]
        {
            $"api/uitleningenapi/leen/{boekId}",
            $"api/uitleningen/leen/{boekId}",
            $"api/uitleningenapi/borrow/{boekId}",
            $"api/uitleningen/borrow/{boekId}",
        };

        foreach (var url in candidates)
        {
            try
            {
                var res = await _http.PostAsync(url, null);
                if (res.IsSuccessStatusCode) return true;

                if (res.StatusCode != HttpStatusCode.NotFound)
                    return false;
            }
            catch
            {
                // Negeer en probeer eventueel volgende URL
            }
        }

        return false;
    }

    // Admin actie: Verwijder een boek via de REST API
    public async Task<bool> AdminBoekDeleteAsync(int id)
    {
        try
        {
            var res = await _http.DeleteAsync($"api/boekenapi/{id}");
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // Helper methode: Voert een HTTP GET request uit en converteert het antwoord (JSON) naar C# objecten
    private async Task<T?> GetAsync<T>(string url)
    {
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"GET {url} failed: {(int)res.StatusCode} ({res.ReasonPhrase})");

        return await res.Content.ReadFromJsonAsync<T>();
    }

    // Helper methode: Voert een veilige HTTP GET uit zonder uitzondering op te werpen bij 404
    private async Task<(bool ok, T? data, HttpStatusCode status)> TryGetAsync<T>(string url)
    {
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return (false, default, res.StatusCode);

        var data = await res.Content.ReadFromJsonAsync<T>();
        return (true, data, res.StatusCode);
    }
}
