using System.ComponentModel.DataAnnotations;

namespace MijnBibliotheekModels.Models;

public class Uitlening : BaseEntity
{
    // FK naar boek
    public int BoekId { get; set; }
    public Boek? Boek { get; set; }

    // FK naar gebruiker
    [Required]
    public string AppUserId { get; set; } = string.Empty;
    public Identity.AppUser? AppUser { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDatum { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime? EindDatum { get; set; }  // verwachte inleverdatum

    public bool IsTeruggebracht { get; set; }
}
