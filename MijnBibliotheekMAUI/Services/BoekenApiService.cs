using System.Net.Http.Json;
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;

namespace MijnBibliotheekMAUI.Services;

public class BoekenApiService
{
    private readonly HttpClient _http;

    public BoekenApiService()
    {
        var handler = new HttpClientHandler();

#if DEBUG
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(ApiSession.GetBaseUrl())
        };
    }

    public BoekenApiService(HttpClient http, ApiSession session)
    {
        _http = http;
        if (_http.BaseAddress == null)
            _http.BaseAddress = new Uri(ApiSession.GetBaseUrl()); 
    }

    public async Task<List<BoekDto>> GetBoekenAsync()
        => await _http.GetFromJsonAsync<List<BoekDto>>("api/boekenapi") ?? new();
}
