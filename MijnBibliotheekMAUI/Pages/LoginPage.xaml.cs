using MijnBibliotheekMAUI.ViewModels;

namespace MijnBibliotheekMAUI.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginVm vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
