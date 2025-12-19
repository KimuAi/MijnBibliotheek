namespace MijnBibliotheekMAUI.Dtos;
// Data Transfer Object (DTO) voor Me 
public class MeDto
{
    public string UserName { get; set; } = "";
    public string FullName { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}
