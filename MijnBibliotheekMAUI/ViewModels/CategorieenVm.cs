
using MijnBibliotheekMAUI.Dtos;
using MijnBibliotheekMAUI.Models;
using MijnBibliotheekMAUI.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MijnBibliotheekMAUI.ViewModels;

// ViewModel voor de categorieënpagina - lijst, toevoegen, bewerken en verwijderen
public class CategorieenVm : BaseVm
{
    private readonly BibliotheekApiService _api;
    private readonly ApiSession _session;

    // De lijst van categorieën die de gebruiker te zien krijgt op het scherm
    public ObservableCollection<CategorieDto> Categorieen { get; } = new();

    // Naam voor een nieuwe categorie (invoerveld binding)
    public string NewCategoryName { get; set; } = string.Empty;

    // Naam voor het bewerken van een bestaande categorie
    private string _editNaam = string.Empty;
    public string EditNaam
    {
        get => _editNaam;
        set { _editNaam = value; OnPropertyChanged(); }
    }

    // De categorie die momenteel bewerkt wordt (null = geen)
    private CategorieDto? _editTarget;
    public CategorieDto? EditTarget
    {
        get => _editTarget;
        set
        {
            _editTarget = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEditing));
        }
    }

    // Geeft aan of er momenteel een categorie aan het bewerken is
    public bool IsEditing => EditTarget != null;

    // Enkel admins kunnen toevoegen, bewerken en verwijderen
    public bool IsAdmin => _session.IsAdmin;

    // Opdrachten voor de knoppen op het scherm
    public ICommand RefreshCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand SaveEditCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand DeleteCommand { get; }

    public CategorieenVm(BibliotheekApiService api, ApiSession session)
    {
        _api = api;
        _session = session;

        // Koppel elk commando aan de juiste methode
        RefreshCommand = new Command(async () => await Load());
        AddCommand = new Command(async () => await Add());
        EditCommand = new Command<CategorieDto>(StartEdit);
        SaveEditCommand = new Command(async () => await SaveEdit());
        CancelEditCommand = new Command(() => { EditTarget = null; EditNaam = ""; });
        DeleteCommand = new Command<CategorieDto>(async (c) => await Delete(c));
    }

    // Klikt men op "Bewerken" dan wordt het invoerveld zichtbaar met de huidige naam
    private void StartEdit(CategorieDto? cat)
    {
        if (cat == null) return;
        EditTarget = cat;
        EditNaam = cat.Naam;
    }

    // Slaat de bewerkte naam op via de REST API
    private async Task SaveEdit()
    {
        if (EditTarget == null) return;
        if (string.IsNullOrWhiteSpace(EditNaam))
        {
            Error = "Naam mag niet leeg zijn";
            return;
        }

        IsBusy = true;
        Error = "";
        try
        {
            // Stuur de aangepaste naam naar de web API
            var ok = await _api.UpdateCategorieAsync(EditTarget.Id, EditNaam.Trim());
            if (!ok)
                Error = "Bewerken mislukt";
            else
            {
                // Verberg het bewerkpaneel en herlaad de lijst
                EditTarget = null;
                EditNaam = "";
                await Load();
            }
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

    // Verwijdert een categorie na bevestiging via de REST API
    private async Task Delete(CategorieDto? cat)
    {
        if (cat == null) return;

        // Vraag bevestiging aan de gebruiker
        bool bevestigd = await Application.Current!.Windows[0].Page!
            .DisplayAlert("Verwijderen", $"Wil je '{cat.Naam}' verwijderen?", "Ja", "Neen");

        if (!bevestigd) return;

        IsBusy = true;
        Error = "";
        try
        {
            // Stuur het verwijderverzoek naar de web API (soft-delete)
            var ok = await _api.DeleteCategorieAsync(cat.Id);
            if (!ok)
                Error = "Verwijderen mislukt";
            else
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

    // Voegt een nieuwe categorie toe via de REST API
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
            var ok = await _api.CreateCategorieAsync(NewCategoryName.Trim());
            if (!ok)
                Error = "Aanmaken mislukt";
            else
            {
                NewCategoryName = "";
                OnPropertyChanged(nameof(NewCategoryName));
                await Load();
            }
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

    // Laadt alle categorieën via de API (met offline fallback via SQLite)
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
