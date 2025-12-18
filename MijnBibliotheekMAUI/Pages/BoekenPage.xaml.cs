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
        if (_vm.Boeken.Count == 0) await _vm.Load();
    }
}
