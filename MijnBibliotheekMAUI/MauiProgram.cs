using Microsoft.Extensions.Logging;
using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Pages;
using MijnBibliotheekMAUI.Services;
using MijnBibliotheekMAUI.ViewModels;
using System.Net;

namespace MijnBibliotheekMAUI;
// maui program startup
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // cookies + user info
        builder.Services.AddSingleton<ApiSession>();

        // HttpClient met CookieContainer Identity cookie
        builder.Services.AddSingleton(sp =>
        {
            var session = sp.GetRequiredService<ApiSession>();

            var handler = new HttpClientHandler
            {
                CookieContainer = session.Cookies,
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.All
            };

#if DEBUG
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiSession.GetBaseUrl())
            };
        });

        // Services
        builder.Services.AddSingleton<AuthApiService>();
        builder.Services.AddSingleton<BibliotheekApiService>();
        builder.Services.AddSingleton<LocalDbService>();

        // ViewModels
        builder.Services.AddSingleton<LoginVm>();
        builder.Services.AddSingleton<BoekenVm>();
        builder.Services.AddSingleton<CategorieenVm>();
        builder.Services.AddSingleton<UitleningenVm>();

        // Pages
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<BoekenPage>();
        builder.Services.AddSingleton<CategorieenPage>();
        builder.Services.AddSingleton<UitleningenPage>();

        // Shell
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}
