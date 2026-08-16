using MijnBibliotheekModels.Models;

namespace MijnBibliotheekWeb.ViewModels;

public class BoekenListViewModel
{
    public List<Boek> Boeken { get; set; } = new();

    public string? SearchQuery { get; set; }
    public int? CategorieId { get; set; }

    public string SortBy { get; set; } = "Titel";
    public string SortOrder { get; set; } = "asc";

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalPages { get; set; } = 1;
    public int TotalItems { get; set; } = 0;

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
