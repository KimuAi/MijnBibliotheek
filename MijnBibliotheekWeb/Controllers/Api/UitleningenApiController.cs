using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers.Api;
// API controller voor uitlening-gerelateerde endpoints
[ApiController]
[Route("api/uitleningenapi")]
public class UitleningenApiController : ControllerBase
{   // Endpoint om alle uitleningen van de ingelogde gebruiker op te halen
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<object>());
    }
    // Endpoint om een boek uit te lenen voor de ingelogde gebruiker
    [Authorize]
    [HttpPost("{boekId:int}/leen")]
    public IActionResult Leen(int boekId)
    {
       
        return Ok(new { Success = true, BoekId = boekId });
    }
    //  Alleen admin-gebruikers kunnen een uitlening als teruggebracht markeren
    [Authorize(Roles = "Admin")]
    [HttpPost("{uitleningId:int}/terug")]
    public IActionResult Terug(int uitleningId)
    {
        return Ok(new { Success = true, UitleningId = uitleningId });
    }
}
