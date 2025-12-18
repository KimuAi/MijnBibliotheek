namespace MijnBibliotheekWeb.Dtos
{
    public class BoekDto
    {
        public int Id { get; set; }
        public string Titel { get; set; } = "";
        public string Auteur { get; set; } = "";
        public string ISBN { get; set; } = "";
        public bool IsBeschikbaar { get; set; }
        public int CategorieId { get; set; }
        public string CategorieNaam { get; set; } = "";
    }

    public class BoekCreateUpdateDto
    {
        public string Titel { get; set; } = "";
        public string Auteur { get; set; } = "";
        public string ISBN { get; set; } = "";
        public bool IsBeschikbaar { get; set; } = true;
        public int CategorieId { get; set; }
    }
}
