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
        // Directe call naar de endpoint (Web API gebruikt User claim)
        return await GetAsync<List<UitleningDto>>("api/uitleningenapi") ?? new();
    }

    public async Task<bool> LeenBoekAsync(int boekId)
    {
        // Correcte route: api/uitleningenapi/{id}/leen
        var url = $"api/uitleningenapi/{boekId}/leen";
        var res = await _http.PostAsync(url, null);
        return res.IsSuccessStatusCode;
    }

    // Admin acties
    public async Task<bool> AdminBoekDeleteAsync(int id)
        => (await _http.DeleteAsync($"api/boekenapi/{id}")).IsSuccessStatusCode;

    public async Task<bool> ReturnBoekAsync(int uitleningId)
    {
        var res = await _http.PostAsync($"api/uitleningenapi/{uitleningId}/terug", null);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> CreateCategorieAsync(string naam)
    {
         var res = await _http.PostAsJsonAsync("api/categorieenapi", new CategorieDto { Naam = naam });
         return res.IsSuccessStatusCode;
    }

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
