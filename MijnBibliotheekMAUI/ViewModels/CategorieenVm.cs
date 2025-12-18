
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MijnBibliotheekMAUI.ViewModels;

public class CategorieenVm : BaseVm
{
    private readonly BibliotheekApiService _api;

    public ObservableCollection<CategorieDto> Categorieen { get; } = new();

    public ICommand RefreshCommand { get; }

    public CategorieenVm(BibliotheekApiService api)
    {
        _api = api;
        RefreshCommand = new Command(async () => await Load());
    }

    public async Task Load()
    {
        Error = "";
        IsBusy = true;

        try
        {
            Categorieen.Clear();
            var data = await _api.GetCategorieenAsync();
            foreach (var c in data) Categorieen.Add(c);
        }
        catch (Exception ex)
        {
            Error = "Kon categorieën niet laden: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
