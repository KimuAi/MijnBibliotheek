using System.Collections.Generic;

namespace MijnBibliotheekWeb.ApiDtos;

/// Info over ingelogde gebruiker
public class MeDto
{
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}