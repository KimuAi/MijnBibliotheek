using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekWPF.Windows;

namespace MijnBibliotheekWPF
{
    public partial class App : Application
    {
        public static IHost HostApp = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var dbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MijnBibliotheek");

            System.IO.Directory.CreateDirectory(dbPath);

            var cs = $"Data Source={System.IO.Path.Combine(dbPath, "bibliotheek.db")}";

            HostApp = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddDbContext<BibliotheekContext>(o => o.UseSqlite(cs));

                    services.AddIdentity<AppUser, IdentityRole>()
                        .AddEntityFrameworkStores<BibliotheekContext>();

                    services.AddScoped<Services.AuthService>();

                    services.AddTransient<LoginWindow>();
                    services.AddTransient<MainWindow>();
                    services.AddTransient<BoekenWindow>();
                    services.AddTransient<CategorienWindow>();
                    services.AddTransient<UitleningWindow>();
                })
                .Build();

            using var scope = HostApp.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BibliotheekContext>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            await BibliotheekSeeder.SeedAsync(db, roleMgr, userMgr);

            HostApp.Services.GetRequiredService<LoginWindow>().Show();
        }
    }
}
