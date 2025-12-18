using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers.Api;

[ApiController]
[Route("api/boekenapi")]
public class BoekenApiController : ControllerBase
{

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<object>());
    }

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
