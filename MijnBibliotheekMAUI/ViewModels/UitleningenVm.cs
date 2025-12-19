
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MijnBibliotheekMAUI.ViewModels;
//uitleningenlijst + inleverfunctie
public class UitleningenVm : BaseVm
{
    private readonly BibliotheekApiService _api;

    public ObservableCollection<UitleningDto> Uitleningen { get; } = new();
    public ICommand RefreshCommand { get; }

    public UitleningenVm(BibliotheekApiService api)
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
            Uitleningen.Clear();
            var data = await _api.GetMijnUitleningenAsync();
            foreach (var u in data) Uitleningen.Add(u);
        }
        catch (Exception ex)
        {
            Error = "Kon uitleningen niet laden: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
