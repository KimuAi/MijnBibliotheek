using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Services;
using System.Windows.Input;

namespace MijnBibliotheekMAUI.ViewModels;
//login + registratie
public class LoginVm : BaseVm
{
    private readonly AuthApiService _auth;
    private readonly IServiceProvider _services;

    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    // Registratie
    public string RegisterFullName { get; set; } = "";
    public string RegisterEmail { get; set; } = "";
    public string RegisterPassword { get; set; } = "";

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    public LoginVm(AuthApiService auth, IServiceProvider services)
    {
        _auth = auth;
        _services = services;

        LoginCommand = new Command(async () => await Login());
        RegisterCommand = new Command(async () => await Register());
    }

    private async Task Login()
    {
        Error = "";
        IsBusy = true;

        try
        {
            var ok = await _auth.LoginAsync(Email, Password);
            if (!ok)
            {
                Error = "Ongeldige login.";
                return;
            }

            // na login, navigeer naar de hoofdpagina
            Application.Current!.MainPage =
                _services.GetRequiredService<AppShell>();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task Register()
    {
        Error = "";
        IsBusy = true;

        try
        {
            var ok = await _auth.RegisterAsync(
                RegisterFullName,
                RegisterEmail,
                RegisterPassword);

            if (!ok)
            {
                Error = "Registratie mislukt.";
                return;
            }

            // Automatisch inloggen na registratie
            await LoginAsyncAfterRegister();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoginAsyncAfterRegister()
    {
        Email = RegisterEmail;
        Password = RegisterPassword;
        await Login();
    }
}
