using System;
using System.ComponentModel.DataAnnotations;

namespace MijnBibliotheekWeb.ViewModels
{
    public class UitleningCreateVm
    {
        [Required]
        public int BoekId { get; set; }

        [Required]
        public string AppUserId { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDatum { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? EindDatum { get; set; } = DateTime.Today.AddDays(14);
    }
}
