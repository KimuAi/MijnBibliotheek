using System.Net;
using System.Net.Http.Json;
using MijnBibliotheekMAUI.Dtos;

namespace MijnBibliotheekMAUI.Services;
/// Service voor bibliotheek-gerelateerde API-aanroepen
public class BibliotheekApiService
{
    private readonly HttpClient _http;

    public BibliotheekApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BoekDto>> GetBoekenAsync()
        => await GetAsync<List<BoekDto>>("api/boekenapi") ?? new();

    public async Task<List<CategorieDto>> GetCategorieenAsync()
        => await GetAsync<List<CategorieDto>>("api/categorieenapi") ?? new();

    //probeert meerdere mogelijke routes
    public async Task<List<UitleningDto>> GetMijnUitleningenAsync()
    {
        var bases = new[] { "api/uitleningenapi", "api/uitleningen" };
        var tails = new[] { "mijn", "mine", "me", "mijnuitleningen", "mijn-uitleningen", "user" };

        foreach (var b in bases)
        {
            foreach (var t in tails)
            {
                var url = $"{b}/{t}";
                var (ok, data, code) = await TryGetAsync<List<UitleningDto>>(url);
                if (ok) return data ?? new();

                if (code != HttpStatusCode.NotFound)
                    throw new HttpRequestException($"GET {url} failed: {(int)code} ({code})");
            }
        }

        throw new HttpRequestException(
            "Uitleningen endpoint niet gevonden. Zoek in browser welke route werkt en pas bases/tails aan.");
    }

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
            var res = await _http.PostAsync(url, null);
            if (res.IsSuccessStatusCode) return true;

            if (res.StatusCode != HttpStatusCode.NotFound)
                return false;
        }

        throw new HttpRequestException("Leen endpoint niet gevonden. Pas candidates aan naar jouw echte route.");
    }

    // Admin acties
    public async Task<bool> AdminBoekDeleteAsync(int id)
        => (await _http.DeleteAsync($"api/boekenapi/{id}")).IsSuccessStatusCode;

    // helper methods

    private async Task<T?> GetAsync<T>(string url)
    {
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"GET {url} failed: {(int)res.StatusCode} ({res.ReasonPhrase})");

        return await res.Content.ReadFromJsonAsync<T>();
    }

    private async Task<(bool ok, T? data, HttpStatusCode status)> TryGetAsync<T>(string url)
    {
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return (false, default, res.StatusCode);

        var data = await res.Content.ReadFromJsonAsync<T>();
        return (true, data, res.StatusCode);
    }
}
