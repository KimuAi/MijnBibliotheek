using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers.Api;

[ApiController]
[Route("api/categorieenapi")]
public class CategorieenApiController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<object>());
    }

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
