namespace MijnBibliotheekMAUI.Dtos;

public class UitleningDto
{
    public int Id { get; set; }
    public int BoekId { get; set; }
    public string BoekTitel { get; set; } = "";
    public DateTime StartDatum { get; set; }
    public DateTime? EindDatum { get; set; }
}
    