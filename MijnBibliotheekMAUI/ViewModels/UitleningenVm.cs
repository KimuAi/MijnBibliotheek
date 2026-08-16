
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
    public ICommand ReturnCommand { get; }

    public UitleningenVm(BibliotheekApiService api)
    {
        _api = api;
        RefreshCommand = new Command(async () => await Load());
        ReturnCommand = new Command<UitleningDto>(async (u) => await Return(u));
    }

    private async Task Return(UitleningDto? u)
    {
        if (u == null) return;
        if (u.IsTeruggebracht) return;

        Error = "";
        IsBusy = true;
        try
        {
            var ok = await _api.ReturnBoekAsync(u.Id);
            if (!ok) Error = "Terugbrengen mislukt.";
            await Load();
        }
        catch (Exception ex)
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
            Uitleningen.Clear();
            
            // Check if user is theoretically logged in (has valid session)
            // But API call will confirm. 
            // Better to show friendly error if we get 401.
            
            var data = await _api.GetMijnUitleningenAsync();
            foreach (var u in data) Uitleningen.Add(u);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("401") || ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Error = "Je bent niet ingelogd. Ga naar de Login pagina.";
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
