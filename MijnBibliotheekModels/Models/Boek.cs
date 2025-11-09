using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MijnBibliotheekModels.Models;

public class Boek : BaseEntity
{
    [Required, MaxLength(150)]
    public string Titel { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Auteur { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;

    public bool IsBeschikbaar { get; set; } = true;

    // FK
    public int CategorieId { get; set; }
    public Categorie? Categorie { get; set; }

    public ICollection<Uitlening> Uitleningen { get; set; } = new List<Uitlening>();
}
