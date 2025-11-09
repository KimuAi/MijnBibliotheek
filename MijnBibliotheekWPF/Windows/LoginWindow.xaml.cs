using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MijnBibliotheekWPF.Services;

namespace MijnBibliotheekWPF.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _auth;

        public LoginWindow(AuthService auth)
        {
            InitializeComponent();
            _auth = auth;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var (ok, msg, user, roles) = await _auth.LoginAsync(EmailBox.Text, PwBox.Password);

            if (!ok)
            {
                StatusText.Text = msg;
                return;
            }

            var main = App.HostApp.Services.GetRequiredService<MainWindow>();
            main.SetCurrentUser(user!, roles);
            main.Show();
            Close();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            var (ok, msg) = await _auth.RegisterAsync(EmailBox.Text, NaamBox.Text, PwBox.Password);
            StatusText.Text = ok ? "✅ Registratie gelukt — Je kan nu inloggen" : msg;
        }
    }
}
