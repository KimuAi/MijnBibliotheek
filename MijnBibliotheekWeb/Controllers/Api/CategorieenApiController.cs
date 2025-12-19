using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers.Api;
// API-controller voor categorie-gerelateerde endpoints
[ApiController]
[Route("api/categorieenapi")]
public class CategorieenApiController : ControllerBase
{   // Publiek endpoint om categorieën op te halen
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<object>());
    }
    //  Alleen admin-gebruikers kunnen categorieën aanmaken, bijwerken en verwijderen
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create([FromBody] object cat) => Ok();

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] object cat) => Ok();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => Ok();
}
