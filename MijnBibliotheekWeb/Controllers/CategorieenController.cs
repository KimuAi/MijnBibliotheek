using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;

namespace MijnBibliotheekWeb.Controllers;

// Controller voor het beheren van boek-categorieën in de bibliotheek
[Authorize]
public class CategorieenController : Controller
{
    private readonly BibliotheekContext _db;
    public CategorieenController(BibliotheekContext db) => _db = db;

    // GET: /Categorieen/Index
    // Haalt alle niet-verwijderde categorieën op uit de database, gesorteerd op naam
    public async Task<IActionResult> Index()
        => View(await _db.Categorieen.Where(c => !c.IsDeleted).OrderBy(c => c.Naam).ToListAsync());

    // POST: /Categorieen/Add
    // Voegt een nieuwe categorie toe aan de databank (alleen voor Admin & Medewerker)
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Add(string naam)
    {
        if (!string.IsNullOrWhiteSpace(naam))
        {
            _db.Categorieen.Add(new Categorie { Naam = naam.Trim() });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Categorie succesvol toegevoegd.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Categorieen/Edit
    // Bewerkt de naam van een categorie (alleen voor Admin & Medewerker)
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, string naam)
    {
        var cat = await _db.Categorieen.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (cat == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(naam))
        {
            cat.Naam = naam.Trim();
            await _db.SaveChangesAsync();
            TempData["Success"] = "Categorie succesvol bijgewerkt.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Categorieen/Delete/5
    // Voert een soft-delete uit op een categorie
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categorieen.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (cat == null) return NotFound();

        cat.IsDeleted = true;
        cat.DeletedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Categorie succesvol verwijderd.";

        return RedirectToAction(nameof(Index));
    }
}
