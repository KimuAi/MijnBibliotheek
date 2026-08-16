namespace MijnBibliotheekWeb.ApiDtos;

/// DTO voor categorieën (API)
public class CategorieDto
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
}