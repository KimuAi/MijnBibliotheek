namespace MijnBibliotheekWeb.Dtos
{
    public class CategorieDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = "";
    }

    public class CategorieCreateUpdateDto
    {
        public string Naam { get; set; } = "";
    }
}
