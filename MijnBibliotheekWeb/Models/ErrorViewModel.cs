namespace MijnBibliotheekWeb.Models
{//  voor het weergeven van foutinformatie in de bibliotheekapplicatie.

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
