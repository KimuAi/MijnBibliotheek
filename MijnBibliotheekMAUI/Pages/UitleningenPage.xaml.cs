using MijnBibliotheekMAUI.ViewModels;

namespace MijnBibliotheekMAUI.Pages;

public partial class UitleningenPage : ContentPage
{
    private readonly UitleningenVm _vm;

    public UitleningenPage(UitleningenVm vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Uitleningen.Count == 0) await _vm.Load();
    }
}
