using System.Net.Http.Json;
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;

namespace MijnBibliotheekMAUI.Services;

public class AuthApiService
{
    private readonly HttpClient _http;
    private readonly ApiSession _session;

    public AuthApiService(HttpClient http, ApiSession session)
    {
        _http = http;
        _session = session;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        if (!res.IsSuccessStatusCode) return false;

        return await RefreshMeAsync();
    }

    public async Task<bool> RegisterAsync(string fullname, string email, string password)
    {
        var res = await _http.PostAsJsonAsync("api/auth/register", new RegisterRequestDto
        {
            FullName = fullname,
            Email = email,
            Password = password
        });

        return res.IsSuccessStatusCode;
    }

    public async Task<bool> LogoutAsync()
    {
        var res = await _http.PostAsync("api/auth/logout", null);
        _session.Clear();
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> RefreshMeAsync()
    {
      
        var me = await _http.GetFromJsonAsync<MeDto>("api/auth/me");
        if (me == null) return false;

        _session.UserName = me.UserName;
        _session.FullName = me.FullName;

     
        _session.Roles = (me.Roles ?? new List<string>()).ToList();

        return true;
    }
}
