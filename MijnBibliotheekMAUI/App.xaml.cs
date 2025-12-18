namespace MijnBibliotheekMAUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public App(IServiceProvider services)
    {
        InitializeComponent();

        Services = services;

        MainPage = services.GetRequiredService<AppShell>();
    }
}
