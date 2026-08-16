<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekWeb.ApiDtos;

namespace MijnBibliotheekWeb.Controllers.Api;

/// API voor boeken (CRUD)
=======
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekWeb.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace MijnBibliotheekWeb.Controllers.Api;

>>>>>>> 841af99e05376b83ff6c2a2cf9b76484d6b0b01b
[ApiController]
[Route("api/boekenapi")]
public class BoekenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;

<<<<<<< HEAD
    public BoekenApiController(BibliotheekContext db) => _db = db;

    // GET: api/boekenapi
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lijst = await _db.Boeken
            .Include(b => b.Categorie)
            .AsNoTracking()
            .OrderBy(b => b.Titel)
            .Select(b => new BoekDto
            {
                Id = b.Id,
                Titel = b.Titel,
                Auteur = b.Auteur,
                ISBN = b.ISBN,
                IsBeschikbaar = b.IsBeschikbaar,
                CategorieId = b.CategorieId,
                CategorieNaam = b.Categorie != null ? b.Categorie.Naam : string.Empty
            })
            .ToListAsync();

        return Ok(lijst);
    }

    // GET: api/boekenapi/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var b = await _db.Boeken.Include(x => x.Categorie).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();

        var dto = new BoekDto
        {
            Id = b.Id,
            Titel = b.Titel,
            Auteur = b.Auteur,
            ISBN = b.ISBN,
            IsBeschikbaar = b.IsBeschikbaar,
            CategorieId = b.CategorieId,
            CategorieNaam = b.Categorie?.Naam ?? string.Empty
        };

        return Ok(dto);
    }

    // POST: api/boekenapi
    [Authorize(Roles = "Admin,Medewerker")]
=======
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
>>>>>>> 841af99e05376b83ff6c2a2cf9b76484d6b0b01b
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BoekDto dto)
    {
        if (dto == null) return BadRequest();

        var boek = new MijnBibliotheekModels.Models.Boek
        {
            Titel = dto.Titel,
            Auteur = dto.Auteur,
            ISBN = dto.ISBN,
            CategorieId = dto.CategorieId,
            IsBeschikbaar = dto.IsBeschikbaar
        };

        _db.Boeken.Add(boek);
        await _db.SaveChangesAsync();

        dto.Id = boek.Id;
        return CreatedAtAction(nameof(GetById), new { id = boek.Id }, dto);
    }

    // PUT: api/boekenapi/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BoekDto dto)
    {
        if (dto == null || id != dto.Id) return BadRequest();

        var boek = await _db.Boeken.FindAsync(id);
        if (boek == null) return NotFound();

        boek.Titel = dto.Titel;
        boek.Auteur = dto.Auteur;
        boek.ISBN = dto.ISBN;
        boek.CategorieId = dto.CategorieId;
        boek.IsBeschikbaar = dto.IsBeschikbaar;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/boekenapi/5 (soft-delete)
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var boek = await _db.Boeken.FindAsync(id);
        if (boek == null) return NotFound();

        boek.IsDeleted = true;
        boek.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
