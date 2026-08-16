using MijnBibliotheekMAUI.ViewModels;

namespace MijnBibliotheekMAUI.Pages;

public partial class BoekenPage : ContentPage
{
    private readonly BoekenVm _vm;

    public BoekenPage(BoekenVm vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Always reload to ensure IsAdmin and IsLoggedIn are updated
        await _vm.Load();
    }
}
