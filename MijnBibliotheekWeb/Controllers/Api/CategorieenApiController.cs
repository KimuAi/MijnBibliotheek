using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekWeb.ApiDtos;

namespace MijnBibliotheekWeb.Controllers.Api;

/// API-controller voor categorieën
[ApiController]
[Route("api/categorieenapi")]
public class CategorieenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;

    public CategorieenApiController(BibliotheekContext db) => _db = db;

    // GET: api/categorieenapi
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lijst = await _db.Categorieen
            .Where(c => !c.IsDeleted)
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
        var cat = await _db.Categorieen.Where(c => !c.IsDeleted).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (cat == null) return NotFound();
        return Ok(new CategorieDto { Id = cat.Id, Naam = cat.Naam });
    }

    // POST: api/categorieenapi
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategorieDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Naam)) return BadRequest();

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

        var cat = await _db.Categorieen.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
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
        var cat = await _db.Categorieen.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (cat == null) return NotFound();

        cat.IsDeleted = true;
        cat.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}