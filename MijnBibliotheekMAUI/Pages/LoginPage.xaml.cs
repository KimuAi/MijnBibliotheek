using MijnBibliotheekMAUI.ViewModels;

namespace MijnBibliotheekMAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginVm _vm;

    public LoginPage(LoginVm vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Clear session on login screen appearance
        await _vm.Logout();
    }
}
