using Microsoft.AspNetCore.Identity;

namespace MijnBibliotheekModels.Identity;

public class AppUser : IdentityUser
{
    // Extra eigenschap 
    public string VolledigeNaam { get; set; } = string.Empty;

    // Blokkeren via rolbeheer
    public bool IsGeblokkeerd { get; set; }
}
