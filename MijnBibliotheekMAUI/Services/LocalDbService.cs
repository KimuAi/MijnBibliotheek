using SQLite;
using MijnBibliotheekMAUI.Dtos;

namespace MijnBibliotheekMAUI.Services;

// SQLite Tabel-definitie voor lokale opslag van Boeken op de mobiele telefoon
[Table("LocalBoeken")]
public class BoekLocal
{
    // PrimaryKey geeft aan dat dit de unieke sleutel is in het lokale SQLite databankje
    [PrimaryKey]
    public int Id { get; set; }
    public string Titel { get; set; } = "";
    public string Auteur { get; set; } = "";
    public string ISBN { get; set; } = "";
    public int CategorieId { get; set; }
    public string? CategorieNaam { get; set; }
    public bool IsBeschikbaar { get; set; }
}

// SQLite Tabel-definitie voor Categorieën
[Table("LocalCategorieen")]
public class CategorieLocal
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Naam { get; set; } = "";
}

// SQLite Tabel-definitie voor Uitleningen
[Table("LocalUitleningen")]
public class UitleningLocal
{
    [PrimaryKey]
    public int Id { get; set; }
    public int BoekId { get; set; }
    public string? BoekTitel { get; set; }
    public DateTime StartDatum { get; set; }
    public DateTime? EindDatum { get; set; }
    public bool IsTeruggebracht { get; set; }
}

// Service klasse die het lokale SQLite databankje van de MAUI app beheert voor offline gebruik
public class LocalDbService
{
    // Verbinding met het lokale SQLite databankje
    private SQLiteAsyncConnection? _db;

    // Initialiseert en maakt de tabellen aan op de telefoon als ze nog niet bestaan
    private async Task InitAsync()
    {
        // Als de verbinding al gemaakt is, hoeven we niks opnieuw aan te maken
        if (_db != null) return;

        // Bepaal het beveiligde bestandspad van de lokale app-data map
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "local_bibliotheek.db3");
        
        // Open de asynchrone SQLite verbinding
        _db = new SQLiteAsyncConnection(dbPath);

        // Maak de tabellen asynchroon aan in de lokale databank
        await _db.CreateTableAsync<BoekLocal>();
        await _db.CreateTableAsync<CategorieLocal>();
        await _db.CreateTableAsync<UitleningLocal>();
    }

    // Slaat de van de API opgehaalde boeken lokaal op (overschrijft de oude cache)
    public async Task SaveBoekenAsync(IEnumerable<BoekDto> boeken)
    {
        await InitAsync();
        
        // Wis oude lokale gegevens
        await _db!.DeleteAllAsync<BoekLocal>();
        
        // Vorm DTOs om naar lokale SQLite objecten
        var list = boeken.Select(b => new BoekLocal
        {
            Id = b.Id,
            Titel = b.Titel,
            Auteur = b.Auteur,
            ISBN = b.ISBN,
            CategorieId = b.CategorieId,
            CategorieNaam = b.CategorieNaam,
            IsBeschikbaar = b.IsBeschikbaar
        }).ToList();

        // Voeg alle boeken in één keer toe aan het lokale SQLite bestand
        await _db.InsertAllAsync(list);
    }

    // Haalt boeken op uit het lokale SQLite bestand wanneer er geen internetverbinding is
    public async Task<List<BoekDto>> GetBoekenAsync()
    {
        await InitAsync();
        
        // Lees alle rijen uit de lokale SQLite tabel
        var list = await _db!.Table<BoekLocal>().ToListAsync();
        
        // Vorm om naar DTOs voor gebruik in de MAUI schermen (ViewModels)
        return list.Select(b => new BoekDto
        {
            Id = b.Id,
            Titel = b.Titel,
            Auteur = b.Auteur,
            ISBN = b.ISBN,
            CategorieId = b.CategorieId,
            CategorieNaam = b.CategorieNaam,
            IsBeschikbaar = b.IsBeschikbaar
        }).ToList();
    }

    // Slaat categorieën lokaal op
    public async Task SaveCategorieenAsync(IEnumerable<CategorieDto> cats)
    {
        await InitAsync();
        await _db!.DeleteAllAsync<CategorieLocal>();
        var list = cats.Select(c => new CategorieLocal
        {
            Id = c.Id,
            Naam = c.Naam
        }).ToList();

        await _db.InsertAllAsync(list);
    }

    // Haalt categorieën op uit de lokale SQLite opslag
    public async Task<List<CategorieDto>> GetCategorieenAsync()
    {
        await InitAsync();
        var list = await _db!.Table<CategorieLocal>().ToListAsync();
        return list.Select(c => new CategorieDto
        {
            Id = c.Id,
            Naam = c.Naam
        }).ToList();
    }

    // Slaat uitleningen lokaal op
    public async Task SaveUitleningenAsync(IEnumerable<UitleningDto> uitleningen)
    {
        await InitAsync();
        await _db!.DeleteAllAsync<UitleningLocal>();
        var list = uitleningen.Select(u => new UitleningLocal
        {
            Id = u.Id,
            BoekId = u.BoekId,
            BoekTitel = u.BoekTitel,
            StartDatum = u.StartDatum,
            EindDatum = u.EindDatum,
            IsTeruggebracht = u.IsTeruggebracht
        }).ToList();

        await _db.InsertAllAsync(list);
    }

    // Haalt uitleningen op uit de lokale SQLite opslag
    public async Task<List<UitleningDto>> GetUitleningenAsync()
    {
        await InitAsync();
        var list = await _db!.Table<UitleningLocal>().ToListAsync();
        return list.Select(u => new UitleningDto
        {
            Id = u.Id,
            BoekId = u.BoekId,
            BoekTitel = u.BoekTitel,
            StartDatum = u.StartDatum,
            EindDatum = u.EindDatum,
            IsTeruggebracht = u.IsTeruggebracht
        }).ToList();
    }
}
