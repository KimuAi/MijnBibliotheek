using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.Dtos;

namespace MijnBibliotheekWeb.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/uitleningenapi")]
public class UitleningenApiController : ControllerBase
{
    private readonly BibliotheekContext _db;
    private readonly UserManager<AppUser> _userMgr;

    public UitleningenApiController(BibliotheekContext db, UserManager<AppUser> userMgr)
    {
        _db = db;
        _userMgr = userMgr;
    }

    [HttpGet]
    public async Task<ActionResult<List<UitleningDto>>> Get()
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        return await _db.Uitleningen
            .Include(u => u.Boek)
            .Where(u => u.AppUserId == user.Id)
            .OrderByDescending(u => u.StartDatum)
            .Select(u => new UitleningDto
            {
                Id = u.Id,
                BoekId = u.BoekId,
                BoekTitel = u.Boek != null ? u.Boek.Titel : "",
                AppUserId = u.AppUserId,
                LenerNaam = user.VolledigeNaam,
                StartDatum = u.StartDatum,
                EindDatum = u.EindDatum,
                IsTeruggebracht = u.IsTeruggebracht
            })
            .ToListAsync();
    }

    [HttpPost("{boekId:int}/leen")]
    public async Task<IActionResult> Leen(int boekId)
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var boek = await _db.Boeken.FindAsync(boekId);
        if (boek == null) return NotFound("Boek niet gevonden");
        
        if (!boek.IsBeschikbaar) return BadRequest("Boek is al uitgeleend");

        var uitlening = new Uitlening
        {
            BoekId = boekId,
            AppUserId = user.Id,
            StartDatum = DateTime.Now,
            EindDatum = DateTime.Now.AddDays(21),
            IsTeruggebracht = false
        };

        boek.IsBeschikbaar = false;
        _db.Uitleningen.Add(uitlening);
        await _db.SaveChangesAsync();

        return Ok(new { Success = true, BoekId = boekId });
    }

    [Authorize] // Allow user to return their own book (implied) or Admin
    [HttpPost("{uitleningId:int}/terug")]
    public async Task<IActionResult> Terug(int uitleningId)
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var uitlening = await _db.Uitleningen.Include(u => u.Boek).FirstOrDefaultAsync(u => u.Id == uitleningId);
        if (uitlening == null) return NotFound("Uitlening niet gevonden");

        // Check ownership or admin
        var isAdmin = await _userMgr.IsInRoleAsync(user, "Admin");
        if (uitlening.AppUserId != user.Id && !isAdmin)
            return Forbid();

        if (uitlening.IsTeruggebracht)
            return BadRequest("Boek is al teruggebracht");

        uitlening.IsTeruggebracht = true;
        uitlening.EindDatum = DateTime.Now; // Set actual return date

        if (uitlening.Boek != null)
        {
            uitlening.Boek.IsBeschikbaar = true;
        }

        await _db.SaveChangesAsync();

        return Ok(new { Success = true, UitleningId = uitleningId });
    }
}
