namespace MijnBibliotheekWeb.ApiDtos;

/// DTO voor boeken (API)
public class BoekDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Auteur { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public bool IsBeschikbaar { get; set; }
    public int CategorieId { get; set; }
    public string CategorieNaam { get; set; } = string.Empty;
}