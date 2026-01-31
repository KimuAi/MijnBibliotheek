using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekWeb.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace MijnBibliotheekWeb.Controllers.Api;

[ApiController]
[Route("api/boekenapi")]
public class BoekenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;

    public BoekenApiController(BibliotheekContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<BoekDto>>> Get()
    {
        return await _db.Boeken.Include(b => b.Categorie)
            .Where(b => !b.IsDeleted)
            .Select(b => new BoekDto
            {
                Id = b.Id,
                Titel = b.Titel,
                Auteur = b.Auteur,
                ISBN = b.ISBN,
                CategorieId = b.CategorieId,
                CategorieNaam = b.Categorie != null ? b.Categorie.Naam : "",
                IsBeschikbaar = b.IsBeschikbaar
            })
            .ToListAsync();
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
