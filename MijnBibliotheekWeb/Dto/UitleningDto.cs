namespace MijnBibliotheekWeb.Dtos
{// DTOs voor uitlening-gerelateerde operaties in de bibliotheekapplicatie.
    public class UitleningDto
    {
        public int Id { get; set; }
        public int BoekId { get; set; }
        public string BoekTitel { get; set; } = "";
        public string AppUserId { get; set; } = "";
        public string LenerNaam { get; set; } = "";
        public DateTime StartDatum { get; set; }
        public DateTime? EindDatum { get; set; }
        public bool IsTeruggebracht { get; set; }
    }

    public class UitleningCreateDto
    {
        // Admin of Medewerker mag voor iemand anders uitlenen.
        // Lidof User  wordt genegeerd en vervangen door token user id.
        public string? AppUserId { get; set; }

        public int BoekId { get; set; }
        public DateTime StartDatum { get; set; } = DateTime.Today;
        public DateTime? EindDatum { get; set; } = DateTime.Today.AddDays(14);
    }
}
