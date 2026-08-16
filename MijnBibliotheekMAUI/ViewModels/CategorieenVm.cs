
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MijnBibliotheekMAUI.ViewModels;
//categorieënlijst
public class CategorieenVm : BaseVm
{
    private readonly BibliotheekApiService _api;

    private readonly ApiSession _session;

    public ObservableCollection<CategorieDto> Categorieen { get; } = new();

    public string NewCategoryName { get; set; } = string.Empty;
    public bool IsAdmin => _session.IsAdmin;

    public ICommand RefreshCommand { get; }
    public ICommand AddCommand { get; }

    public CategorieenVm(BibliotheekApiService api, ApiSession session)
    {
        _api = api;
        _session = session;
        RefreshCommand = new Command(async () => await Load());
        AddCommand = new Command(async () => await Add());
    }

    private async Task Add()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
             Error = "Naam is verplicht";
             return;
        }
        
        IsBusy = true;
        Error = "";
        try
        {
             var ok = await _api.CreateCategorieAsync(NewCategoryName);
             if(!ok) Error = "Aanmaken mislukt";
             NewCategoryName = "";
             OnPropertyChanged(nameof(NewCategoryName));
             await Load();
        }
        catch(Exception ex)
        {
             Error = ex.Message;
        }
        finally
        {
             IsBusy = false;
        }
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
