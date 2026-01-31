using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.Dtos;

namespace MijnBibliotheekWeb.Controllers.Api;
// API-controller voor categorie-gerelateerde endpoints
[ApiController]
[Route("api/categorieenapi")]
public class CategorieenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;

    public CategorieenApiController(BibliotheekContext db)
    {
        _db = db;
    }

    // Publiek endpoint om categorieën op te halen
    [HttpGet]
    public async Task<ActionResult<List<CategorieDto>>> Get()
    {
        return await _db.Categorieen.Select(c => new CategorieDto
        {
            Id = c.Id,
            Naam = c.Naam
        }).ToListAsync();
    }

    //  Alleen admin-gebruikers kunnen categorieën aanmaken
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategorieDto catDto)
    {
        if (string.IsNullOrWhiteSpace(catDto.Naam))
            return BadRequest("Naam is verplicht");

        var cat = new Categorie { Naam = catDto.Naam };
        _db.Categorieen.Add(cat);
        await _db.SaveChangesAsync();
        
        return Ok(new CategorieDto { Id = cat.Id, Naam = cat.Naam });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] object cat) => Ok();

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => Ok();
}
