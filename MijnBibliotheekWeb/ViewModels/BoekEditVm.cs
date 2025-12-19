using System.ComponentModel.DataAnnotations;

namespace MijnBibliotheekWeb.ViewModels;
/// ViewModel voor het bewerken van een boek
public class BoekEditVm
{
    public int Id { get; set; }

    [Required]
    public string Titel { get; set; } = "";

    [Required]
    public string Auteur { get; set; } = "";

    [Required]
    public string ISBN { get; set; } = "";

    [Required]
    public int CategorieId { get; set; }

    public bool IsBeschikbaar { get; set; } = true;
}
