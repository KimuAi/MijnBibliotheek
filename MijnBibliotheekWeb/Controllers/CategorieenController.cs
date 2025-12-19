using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;

namespace MijnBibliotheekWeb.Controllers;
// Controller voor het beheren van categorieën in de bibliotheekapplicatie.
[Authorize]
public class CategorieenController : Controller
{
    private readonly BibliotheekContext _db;
    public CategorieenController(BibliotheekContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Categorieen.OrderBy(c => c.Naam).ToListAsync());
    // Voegt een nieuwe categorie toe aan de database.
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Add(string naam)
    {
        if (!string.IsNullOrWhiteSpace(naam))
        {
            _db.Categorieen.Add(new Categorie { Naam = naam.Trim() });
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    // Verwijdert een categorie door deze als verwijderd te markeren.
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categorieen.FindAsync(id);
        if (cat == null) return NotFound();

        cat.IsDeleted = true;
        cat.DeletedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
