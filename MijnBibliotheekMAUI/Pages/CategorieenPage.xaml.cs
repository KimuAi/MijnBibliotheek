using MijnBibliotheekMAUI.ViewModels;

namespace MijnBibliotheekMAUI.Pages;

public partial class CategorieenPage : ContentPage
{
    private readonly CategorieenVm _vm;

    public CategorieenPage(CategorieenVm vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}
