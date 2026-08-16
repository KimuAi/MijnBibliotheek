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

    // Publiek: lijst categorieën ophalen
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

    // Alleen Admin: nieuwe categorie aanmaken
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategorieDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Naam))
            return BadRequest();

        var cat = new MijnBibliotheekModels.Models.Categorie { Naam = dto.Naam.Trim() };
        _db.Categorieen.Add(cat);
        await _db.SaveChangesAsync();

        dto.Id = cat.Id;
        return CreatedAtAction(nameof(Get), new { id = cat.Id }, dto);
    }

    // Alleen Admin: categorie updaten
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategorieDto dto)
    {
        var cat = await _db.Categorieen.FindAsync(id);
        if (cat == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Naam)) return BadRequest();

        cat.Naam = dto.Naam.Trim();
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Alleen Admin: soft-delete
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