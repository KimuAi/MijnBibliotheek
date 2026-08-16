namespace MijnBibliotheekMAUI.Dtos;

/// Data Transfer Object voor Uitlening
public class UitleningDto
{
    public int Id { get; set; }
    public int BoekId { get; set; }
    public string BoekTitel { get; set; } = "";
    public DateTime StartDatum { get; set; }
<<<<<<< HEAD
    public DateTime? EindDatum { get; set; }
=======
    public DateTime EindDatum { get; set; }
>>>>>>> 841af99e05376b83ff6c2a2cf9b76484d6b0b01b
    public bool IsTeruggebracht { get; set; }
}