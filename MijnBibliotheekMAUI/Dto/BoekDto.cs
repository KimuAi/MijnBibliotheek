namespace MijnBibliotheekMAUI.Dtos;

public class BoekDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = "";
    public string Auteur { get; set; } = "";
    public string ISBN { get; set; } = "";
    public int CategorieId { get; set; }
    public string? CategorieNaam { get; set; }
    public bool IsBeschikbaar { get; set; }
}
