using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace MijnBibliotheekWeb.Services;

// Eenvoudige service klasse voor meertalige weergave (Nederlands & Engels) in de webapplicatie
public class LanguageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LanguageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Eigenschap die de momenteel gekozen taal/cultuur ophaalt uit de cookie ("_culture")
    public string CurrentCulture
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            // Als de "_culture" cookie aanwezig is op het verzoek, gebruik die waarde
            if (context != null && context.Request.Cookies.TryGetValue("_culture", out var cul) && !string.IsNullOrWhiteSpace(cul))
            {
                return cul.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? "en-US" : "nl-BE";
            }
            // Standaard altijd Nederlands (nl-BE)
            return "nl-BE";
        }
    }

    // Handige hulpeigenschap om te controleren of de actieve taal Engels is
    public bool IsEnglish => CurrentCulture.StartsWith("en", StringComparison.OrdinalIgnoreCase);

    // Vertaalt een tekst-sleutel (bijv. "MenuBoeken") naar de gekozen taal (NL of EN)
    public string GetText(string key)
    {
        if (IsEnglish && _en.TryGetValue(key, out var enText))
            return enText;

        if (_nl.TryGetValue(key, out var nlText))
            return nlText;

        return key;
    }

    // Woordenboek met alle Nederlandse vertalingen voor knoppen, menu's en tabellen
    private static readonly Dictionary<string, string> _nl = new()
    {
        { "BrandTitle", "Mijn Bibliotheek" },
        { "MenuBoeken", "Boeken" },
        { "MenuCategorieen", "Categorieën" },
        { "MenuUitleningen", "Uitleningen" },
        { "SearchPlaceholder", "Zoek op titel, auteur of ISBN..." },
        { "AllCategories", "Alle categorieën" },
        { "FilterButton", "Filteren" },
        { "NewBookButton", "Nieuw boek" },
        { "TableTitle", "Titel" },
        { "TableAuthor", "Auteur" },
        { "TableISBN", "ISBN" },
        { "TableCategory", "Categorie" },
        { "TableStatus", "Status" },
        { "TableActions", "Acties" },
        { "Available", "Beschikbaar" },
        { "Borrowed", "Uitgeleend" },
        { "Edit", "Bewerken" },
        { "Delete", "Verwijderen" },
        { "QuickToggle", "Status Wijzigen (AJAX)" },
        { "Previous", "Vorige" },
        { "Next", "Volgende" },
        { "Page", "Pagina" },
        { "Of", "van" },
        { "TotalItems", "totaal boeken" },
        { "Language", "Taal" },
        { "Dutch", "Nederlands" },
        { "English", "English" },
        { "ConfirmDelete", "Weet je zeker dat je dit boek wilt verwijderen?" },
        { "NoBooksFound", "Geen boeken gevonden." }
    };

    // Woordenboek met alle Engelse vertalingen
    private static readonly Dictionary<string, string> _en = new()
    {
        { "BrandTitle", "My Library" },
        { "MenuBoeken", "Books" },
        { "MenuCategorieen", "Categories" },
        { "MenuUitleningen", "Loans" },
        { "SearchPlaceholder", "Search by title, author or ISBN..." },
        { "AllCategories", "All categories" },
        { "FilterButton", "Filter" },
        { "NewBookButton", "New Book" },
        { "TableTitle", "Title" },
        { "TableAuthor", "Author" },
        { "TableISBN", "ISBN" },
        { "TableCategory", "Category" },
        { "TableStatus", "Status" },
        { "TableActions", "Actions" },
        { "Available", "Available" },
        { "Borrowed", "Borrowed" },
        { "Edit", "Edit" },
        { "Delete", "Delete" },
        { "QuickToggle", "Toggle Status (AJAX)" },
        { "Previous", "Previous" },
        { "Next", "Next" },
        { "Page", "Page" },
        { "Of", "of" },
        { "TotalItems", "total books" },
        { "Language", "Language" },
        { "Dutch", "Nederlands" },
        { "English", "English" },
        { "ConfirmDelete", "Are you sure you want to delete this book?" },
        { "NoBooksFound", "No books found." }
    };
}
