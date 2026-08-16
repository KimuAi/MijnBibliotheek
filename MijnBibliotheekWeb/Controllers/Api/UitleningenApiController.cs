using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.ApiDtos;

namespace MijnBibliotheekWeb.Controllers.Api;

/// API voor uitleningen (mijn uitleningen, lenen, terugbrengen)
[ApiController]
[Route("api/uitleningenapi")]
public class UitleningenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;

    public UitleningenApiController(BibliotheekContext db) => _db = db;

    // GET: api/uitleningenapi/mijn
    [Authorize]
    [HttpGet("mijn")]
    public async Task<IActionResult> GetMijn()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var lijst = await _db.Uitleningen
            .Where(u => u.AppUserId == userId)
            .Include(u => u.Boek)
            .AsNoTracking()
            .OrderByDescending(u => u.StartDatum)
            .Select(u => new UitleningDto
            {
                Id = u.Id,
                BoekId = u.BoekId,
                BoekTitel = u.Boek != null ? u.Boek.Titel : string.Empty,
                AppUserId = u.AppUserId,
                AppUserNaam = string.Empty,
                StartDatum = u.StartDatum,
                EindDatum = u.EindDatum,
                IsTeruggebracht = u.IsTeruggebracht
            })
            .ToListAsync();

        return Ok(lijst);
    }

    // POST: api/uitleningenapi/leen/5
    [Authorize]
    [HttpPost("leen/{boekId:int}")]
    public async Task<IActionResult> Leen(int boekId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var boek = await _db.Boeken.FindAsync(boekId);
        if (boek == null) return NotFound();

        if (!boek.IsBeschikbaar) return BadRequest(new { message = "Boek niet beschikbaar" });

        boek.IsBeschikbaar = false;

        var uit = new Uitlening
        {
            BoekId = boekId,
            AppUserId = userId,
            StartDatum = DateTime.UtcNow,
            IsTeruggebracht = false
        };

        _db.Uitleningen.Add(uit);
        await _db.SaveChangesAsync();

        return Ok(new { success = true, uitleningId = uit.Id });
    }

    // POST: api/uitleningenapi/terug/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost("terug/{id:int}")]
    public async Task<IActionResult> Terug(int id)
    {
        var uit = await _db.Uitleningen.Include(u => u.Boek).FirstOrDefaultAsync(u => u.Id == id);
        if (uit == null) return NotFound();

        if (!uit.IsTeruggebracht)
        {
            uit.IsTeruggebracht = true;
            uit.EindDatum = DateTime.UtcNow;
            if (uit.Boek != null) uit.Boek.IsBeschikbaar = true;
            await _db.SaveChangesAsync();
        }

        return Ok();
    }
}