using MijnBibliotheekMAUI.Pages;
using MijnBibliotheekMAUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MijnBibliotheekMAUI;

public partial class AppShell : Shell
{
    private readonly AuthApiService _auth;

    public AppShell(AuthApiService auth)
    {
        InitializeComponent();
        _auth = auth;

        // simpele logout knop 
        FlyoutItem flyout = new FlyoutItem { Title = "Account" };
        flyout.Items.Add(new ShellContent
        {
            Title = "Logout",
            ContentTemplate = new DataTemplate(() =>
            {
                var btn = new Button { Text = "Logout" };
                btn.Clicked += async (_, __) =>
                {
                    await _auth.LogoutAsync();

                   
                    var services = Application.Current?.Handler?.MauiContext?.Services;
                    var loginPage = services?.GetRequiredService<LoginPage>();

                    if (loginPage != null)
                        Application.Current!.MainPage = new NavigationPage(loginPage);
                };

                return new ContentPage
                {
                    Title = "Logout",
                    Content = new VerticalStackLayout
                    {
                        Padding = 20,
                        Children = { btn }
                    }
                };
            })
        });
        Items.Add(flyout);
    }

    // logout (bovenaan)
    private async void Logout_Clicked(object sender, EventArgs e)
    {
        await _auth.LogoutAsync();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        var loginPage = services?.GetRequiredService<LoginPage>();

        if (loginPage != null)
            Application.Current!.MainPage = new NavigationPage(loginPage);
    }
}
