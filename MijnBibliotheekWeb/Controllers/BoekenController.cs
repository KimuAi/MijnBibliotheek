using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.ViewModels;

namespace MijnBibliotheekWeb.Controllers;
// Controller voor het beheren van boeken in de bibliotheek
[Authorize]
public class BoekenController : Controller
{
    private readonly BibliotheekContext _db;

    public BoekenController(BibliotheekContext db) => _db = db;

    public async Task<IActionResult> Index(string? q = null, int? catId = null)
    {
        var query = _db.Boeken.Include(b => b.Categorie).AsQueryable();

        if (catId.HasValue)
            query = query.Where(b => b.CategorieId == catId.Value);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var t = q.Trim().ToLower();
            query = query.Where(b =>
                b.Titel.ToLower().Contains(t) ||
                b.Auteur.ToLower().Contains(t) ||
                b.ISBN.ToLower().Contains(t));
        }

        ViewBag.Cats = new SelectList(await _db.Categorieen.OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam");
        ViewBag.Q = q;
        ViewBag.CatId = catId;

        return View(await query.OrderBy(b => b.Titel).ToListAsync());
    }
    // GET: Boeken/Create
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Cats = new SelectList(await _db.Categorieen.OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam");
        return View(new BoekEditVm());
    }
    // POST: Boeken/Create
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Create(BoekEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cats = new SelectList(await _db.Categorieen.OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam");
            return View(vm);
        }

        _db.Boeken.Add(new Boek
        {
            Titel = vm.Titel,
            Auteur = vm.Auteur,
            ISBN = vm.ISBN,
            CategorieId = vm.CategorieId,
            IsBeschikbaar = vm.IsBeschikbaar
        });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // GET: Boeken/Edit/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var boek = await _db.Boeken.FindAsync(id);
        if (boek == null) return NotFound();

        ViewBag.Cats = new SelectList(await _db.Categorieen.OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam");

        return View(new BoekEditVm
        {
            Id = boek.Id,
            Titel = boek.Titel,
            Auteur = boek.Auteur,
            ISBN = boek.ISBN,
            CategorieId = boek.CategorieId,
            IsBeschikbaar = boek.IsBeschikbaar
        });
    }
    //  POST: Boeken/Edit/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Edit(BoekEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cats = new SelectList(await _db.Categorieen.OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam");
            return View(vm);
        }

        var boek = await _db.Boeken.FindAsync(vm.Id);
        if (boek == null) return NotFound();

        boek.Titel = vm.Titel;
        boek.Auteur = vm.Auteur;
        boek.ISBN = vm.ISBN;
        boek.CategorieId = vm.CategorieId;
        boek.IsBeschikbaar = vm.IsBeschikbaar;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // POST: Boeken/Delete/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var boek = await _db.Boeken.FindAsync(id);
        if (boek == null) return NotFound();

        boek.IsDeleted = true;
        boek.DeletedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
