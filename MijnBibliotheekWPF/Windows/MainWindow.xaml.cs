using System.Collections.Generic;
using System.Windows;
using MijnBibliotheekModels.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MijnBibliotheekWPF.Windows
{
    public partial class MainWindow : Window
    {
        public IList<string> CurrentRoles { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetCurrentUser(AppUser user, IList<string> roles)
        {
            CurrentRoles = roles;
            RoleText.Text = $"Ingelogd als: {user.VolledigeNaam} ({string.Join(", ", roles)})";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = App.HostApp.Services.GetRequiredService<LoginWindow>();
            login.Show();
            Close();
        }

        private void OpenBoeken(object sender, RoutedEventArgs e)
        {
            var wnd = App.HostApp.Services.GetRequiredService<BoekenWindow>();
            wnd.SetPermissions(CurrentRoles);
            wnd.Show();
        }

        private void OpenCategorieen(object sender, RoutedEventArgs e)
        {
            var wnd = App.HostApp.Services.GetRequiredService<CategorienWindow>();
            wnd.SetPermissions(CurrentRoles);
            wnd.Show();
        }

        private void OpenUitleningen(object sender, RoutedEventArgs e)
        {
            var wnd = App.HostApp.Services.GetRequiredService<UitleningWindow>();
            wnd.SetPermissions(CurrentRoles);
            wnd.Show();
        }
    }
}
