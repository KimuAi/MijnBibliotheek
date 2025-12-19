namespace MijnBibliotheekWeb.Dtos
{// DTOs voor categorie-gerelateerde operaties in de bibliotheekapplicatie.
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
