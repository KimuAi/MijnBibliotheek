using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers.Api;

[ApiController]
[Route("api/uitleningenapi")]
public class UitleningenApiController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<object>());
    }

    [Authorize]
    [HttpPost("{boekId:int}/leen")]
    public IActionResult Leen(int boekId)
    {
       
        return Ok(new { Success = true, BoekId = boekId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{uitleningId:int}/terug")]
    public IActionResult Terug(int uitleningId)
    {
        return Ok(new { Success = true, UitleningId = uitleningId });
    }
}
