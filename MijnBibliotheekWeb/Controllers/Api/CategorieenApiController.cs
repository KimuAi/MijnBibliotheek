using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
<<<<<<< HEAD
using MijnBibliotheekWeb.ApiDtos;
=======
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.Dtos;
>>>>>>> 841af99e05376b83ff6c2a2cf9b76484d6b0b01b

namespace MijnBibliotheekWeb.Controllers.Api;

/// API-controller voor categorieën
[ApiController]
[Route("api/categorieenapi")]
public class CategorieenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;

<<<<<<< HEAD
    public CategorieenApiController(BibliotheekContext db) => _db = db;

    // GET: api/categorieenapi
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lijst = await _db.Categorieen
            .AsNoTracking()
            .OrderBy(c => c.Naam)
            .Select(c => new CategorieDto { Id = c.Id, Naam = c.Naam })
            .ToListAsync();

        return Ok(lijst);
    }

    // GET: api/categorieenapi/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cat = await _db.Categorieen.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (cat == null) return NotFound();
        return Ok(new CategorieDto { Id = cat.Id, Naam = cat.Naam });
    }

    // POST: api/categorieenapi
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategorieDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Naam)) return BadRequest();
=======
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
>>>>>>> 841af99e05376b83ff6c2a2cf9b76484d6b0b01b

        var cat = new MijnBibliotheekModels.Models.Categorie { Naam = dto.Naam.Trim() };
        _db.Categorieen.Add(cat);
        await _db.SaveChangesAsync();

        dto.Id = cat.Id;
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, dto);
    }

    // PUT: api/categorieenapi/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategorieDto dto)
    {
        if (dto == null || id != dto.Id) return BadRequest();

        var cat = await _db.Categorieen.FindAsync(id);
        if (cat == null) return NotFound();

        cat.Naam = dto.Naam.Trim();
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/categorieenapi/5 (soft-delete)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categorieen.FindAsync(id);
        if (cat == null) return NotFound();

        cat.IsDeleted = true;
        cat.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}