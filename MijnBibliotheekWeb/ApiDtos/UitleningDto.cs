using System;

namespace MijnBibliotheekWeb.ApiDtos;

/// DTO voor uitleningen (API)
public class UitleningDto
{
    public int Id { get; set; }
    public int BoekId { get; set; }
    public string BoekTitel { get; set; } = string.Empty;
    public string AppUserId { get; set; } = string.Empty;
    public string AppUserNaam { get; set; } = string.Empty;
    public DateTime StartDatum { get; set; }
    public DateTime? EindDatum { get; set; }
    public bool IsTeruggebracht { get; set; }
}