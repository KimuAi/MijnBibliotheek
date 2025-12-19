using System.Collections.ObjectModel;
using System.Windows.Input;
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Services;

namespace MijnBibliotheekMAUI.ViewModels;
//boekenlijst + leenfunctie + admin delete functie
public class BoekenVm : BaseVm
{
    private readonly BibliotheekApiService _api;
    private readonly ApiSession _session;

    public ObservableCollection<BoekDto> Boeken { get; } = new();

    public bool IsAdmin => _session.IsAdmin;

    public ICommand RefreshCommand { get; }
    public ICommand LeenCommand { get; }
    public ICommand DeleteCommand { get; }

    public BoekenVm(BibliotheekApiService api, ApiSession session)
    {
        _api = api;
        _session = session;

        RefreshCommand = new Command(async () => await Load());
        LeenCommand = new Command<BoekDto>(async (b) => await Leen(b));
        DeleteCommand = new Command<BoekDto>(async (b) => await Delete(b));
    }

    public async Task Load()
    {
        Error = "";
        IsBusy = true;

        try
        {
            Boeken.Clear();
            var data = await _api.GetBoekenAsync();
            foreach (var b in data) Boeken.Add(b);

            OnPropertyChanged(nameof(IsAdmin));
        }
        catch (Exception ex)
        {
            Error = "Kon boeken niet laden: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task Leen(BoekDto? boek)
    {
        if (boek == null) return;

        Error = "";
        try
        {
            var ok = await _api.LeenBoekAsync(boek.Id);
            if (!ok) Error = "Uitlenen mislukt (ingelogd? boek beschikbaar?).";
            await Load();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    private async Task Delete(BoekDto? boek)
    {
        if (boek == null) return;
        if (!IsAdmin) return;

        Error = "";
        try
        {
            var ok = await _api.AdminBoekDeleteAsync(boek.Id);
            if (!ok) Error = "Delete mislukt (endpoint/role?).";
            await Load();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}
