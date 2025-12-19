using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.ViewModels;

namespace MijnBibliotheekWeb.Controllers;
// Controller voor het beheren van uitleningen in de bibliotheekapplicatie.
[Authorize]
public class UitleningenController : Controller
{
    private readonly BibliotheekContext _db;

    public UitleningenController(BibliotheekContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var list = await _db.Uitleningen
            .Include(u => u.Boek)
            .Include(u => u.AppUser)
            .OrderByDescending(u => u.StartDatum)
            .ToListAsync();

        return View(list);
    }
    // Toont het formulier voor het aanmaken van een nieuwe uitlening.
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Boeken = new SelectList(
            await _db.Boeken.Where(b => b.IsBeschikbaar).OrderBy(b => b.Titel).ToListAsync(),
            "Id", "Titel");

        ViewBag.Users = new SelectList(
            await _db.Users.OrderBy(u => u.VolledigeNaam).ToListAsync(),
            "Id", "VolledigeNaam");

        return View(new UitleningCreateVm());
    }
    // Verwerkt het indienen van het formulier voor het aanmaken van een nieuwe uitlening.
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Create(UitleningCreateVm vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Boeken = new SelectList(
                await _db.Boeken.Where(b => b.IsBeschikbaar).OrderBy(b => b.Titel).ToListAsync(),
                "Id", "Titel");

            ViewBag.Users = new SelectList(
                await _db.Users.OrderBy(u => u.VolledigeNaam).ToListAsync(),
                "Id", "VolledigeNaam");

            return View(vm);
        }

        var boek = await _db.Boeken.FindAsync(vm.BoekId);
        if (boek == null || !boek.IsBeschikbaar)
        {
            ModelState.AddModelError("", "Boek is niet beschikbaar.");
            return RedirectToAction(nameof(Create));
        }

        boek.IsBeschikbaar = false;

        _db.Uitleningen.Add(new Uitlening
        {
            BoekId = vm.BoekId,
            AppUserId = vm.AppUserId,
            StartDatum = vm.StartDatum,
            EindDatum = vm.EindDatum,
            IsTeruggebracht = false
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // Markeert een uitlening als teruggebracht.
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Terug(int id)
    {
        var uit = await _db.Uitleningen.Include(x => x.Boek).FirstOrDefaultAsync(x => x.Id == id);
        if (uit == null) return NotFound();

        if (!uit.IsTeruggebracht)
        {
            uit.IsTeruggebracht = true;
            if (uit.Boek != null) uit.Boek.IsBeschikbaar = true;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
