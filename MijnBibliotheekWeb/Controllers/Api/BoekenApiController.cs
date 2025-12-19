using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers.Api;
// API-controller voor boek-gerelateerde endpoints
[ApiController]
[Route("api/boekenapi")]
public class BoekenApiController : ControllerBase
{
    // Publiek endpoint om boeken op te halen
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<object>());
    }
    // Alleen admin-gebruikers kunnen boeken aanmaken, bijwerken en verwijderen
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create([FromBody] object boek) => Ok();

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] object boek) => Ok();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => Ok();
}
