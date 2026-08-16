using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.ViewModels;

namespace MijnBibliotheekWeb.Controllers;

// Controller voor het registreren en opvolgen van uitleningen
[Authorize]
public class UitleningenController : Controller
{
    private readonly BibliotheekContext _db;

    public UitleningenController(BibliotheekContext db) => _db = db;

    // GET: /Uitleningen/Index
    // Toont een overzicht van alle uitleningen met het gekoppelde boek en het ingelogde lid
    public async Task<IActionResult> Index()
    {
        var list = await _db.Uitleningen
            .Include(u => u.Boek)
            .Include(u => u.AppUser)
            .OrderByDescending(u => u.StartDatum)
            .ToListAsync();

        return View(list);
    }

    // GET: /Uitleningen/Create
    // Toont het formulier om een nieuw boek uit te lenen aan een lid
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Alleen beschikbare boeken tonen in de dropdown
        ViewBag.Boeken = new SelectList(
            await _db.Boeken.Where(b => b.IsBeschikbaar && !b.IsDeleted).OrderBy(b => b.Titel).ToListAsync(),
            "Id", "Titel");

        // Alle geregistreerde leden tonen in de dropdown
        ViewBag.Users = new SelectList(
            await _db.Users.OrderBy(u => u.VolledigeNaam).ToListAsync(),
            "Id", "VolledigeNaam");

        return View(new UitleningCreateVm());
    }

    // POST: /Uitleningen/Create
    // Slaat de nieuwe uitlening op in de databank en zet de status van het boek op niet-beschikbaar
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Create(UitleningCreateVm vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Boeken = new SelectList(
                await _db.Boeken.Where(b => b.IsBeschikbaar && !b.IsDeleted).OrderBy(b => b.Titel).ToListAsync(),
                "Id", "Titel");

            ViewBag.Users = new SelectList(
                await _db.Users.OrderBy(u => u.VolledigeNaam).ToListAsync(),
                "Id", "VolledigeNaam");

            return View(vm);
        }

        var boek = await _db.Boeken.FirstOrDefaultAsync(b => b.Id == vm.BoekId && !b.IsDeleted);
        if (boek == null || !boek.IsBeschikbaar)
        {
            TempData["Error"] = "Dit boek is momenteel niet beschikbaar om uit te lenen.";
            return RedirectToAction(nameof(Create));
        }

        // Het boek is nu uitgeleend, dus markeer als niet beschikbaar
        boek.IsBeschikbaar = false;

        // Maak de nieuwe uitlening aan
        _db.Uitleningen.Add(new Uitlening
        {
            BoekId = vm.BoekId,
            AppUserId = vm.AppUserId,
            StartDatum = vm.StartDatum,
            EindDatum = vm.EindDatum,
            IsTeruggebracht = false
        });

        await _db.SaveChangesAsync();
        TempData["Success"] = "Uitlening succesvol geregistreerd.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Uitleningen/Terug/5
    // Registreert het terugbrengen van een boek en maakt het boek weer beschikbaar
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
            TempData["Success"] = "Boek succesvol geregistreerd als teruggebracht.";
        }

        return RedirectToAction(nameof(Index));
    }
}
