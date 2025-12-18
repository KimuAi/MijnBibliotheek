using MijnBibliotheekMAUI.Services;

namespace MijnBibliotheekMAUI;

public partial class MainPage : ContentPage
{
    private readonly BoekenApiService _api;
    private readonly IServiceProvider _sp;

    public MainPage(BoekenApiService api, IServiceProvider sp)
    {
        InitializeComponent();
        _api = api;
        _sp = sp;

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var data = await _api.GetBoekenAsync();
            BoekenList.ItemsSource = data;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout",
                "Kon geen verbinding maken met de API.\n\n" +
                "Check:\n- Web project draait?\n- juiste poort?\n- endpoint bestaat?\n\n" +
                $"Details: {ex.Message}",
                "OK");
        }
    }

    private async void Refresh_Clicked(object sender, EventArgs e)
        => await LoadAsync();

    private async void OpenCategorieen_Clicked(object sender, EventArgs e)
    {
        // ✅ Page via DI → ctor dependencies worden automatisch ingevuld
        var page = _sp.GetRequiredService<Pages.CategorieenPage>();
        await Navigation.PushAsync(page);
    }
}
