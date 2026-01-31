using SQLite;
using MijnBibliotheekMAUI.Dtos;

namespace MijnBibliotheekMAUI.Services;

public class LocalDbService
{
    private SQLiteAsyncConnection? _db;

    public LocalDbService()
    {
    }

    private async Task Init()
    {
        if (_db is not null)
            return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MijnBibliotheek.db3");

        _db = new SQLiteAsyncConnection(databasePath);
        
        // Tabellen aanmaken
        await _db.CreateTableAsync<LocalBoek>();
    }

    public async Task<List<LocalBoek>> GetBoeken()
    {
        await Init();
        if (_db == null) return new List<LocalBoek>();
        return await _db.Table<LocalBoek>().ToListAsync();
    }

    public async Task SaveBoeken(IEnumerable<BoekDto> boeken)
    {
        await Init();
        if (_db != null)
        {
            await _db.DeleteAllAsync<LocalBoek>();
            var localBoeken = boeken.Select(b => new LocalBoek
            {
                Id = b.Id,
                Titel = b.Titel,
                Auteur = b.Auteur,
                ISBN = b.ISBN,
                CategorieNaam = b.CategorieNaam,
                IsBeschikbaar = b.IsBeschikbaar
            });
            await _db.InsertAllAsync(localBoeken);
        }
    }
}

// Lokale versie van BoekDto voor SQLite
public class LocalBoek
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string? CategorieNaam { get; set; }
    public bool IsBeschikbaar { get; set; }
}
