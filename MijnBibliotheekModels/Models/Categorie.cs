using System.ComponentModel.DataAnnotations;

namespace MijnBibliotheekModels.Models;

public class Categorie : BaseEntity
{
    [Required, MaxLength(80)]
    public string Naam { get; set; } = string.Empty;

    public ICollection<Boek> Boeken { get; set; } = new List<Boek>();
}
