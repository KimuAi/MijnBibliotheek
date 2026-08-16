using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Models;
using MijnBibliotheekWeb.ViewModels;

namespace MijnBibliotheekWeb.Controllers;

// [Authorize] zorgt ervoor dat alleen ingelogde gebruikers toegang hebben tot deze controller
[Authorize]
public class BoekenController : Controller
{
    // Databank context veld om te communiceren met de SQLite database
    private readonly BibliotheekContext _db;

    // Dependency Injection: De DbContext wordt automatisch ingespoten via de constructor
    public BoekenController(BibliotheekContext db) => _db = db;

    // GET: /Boeken/Index
    // Deze methode toont de hoofdpagina met boeken, inclusief zoeken (q), categorie filter (catId), sorteren en paginering
    public async Task<IActionResult> Index(
        string? q = null,
        int? catId = null,
        string sortBy = "Titel",
        string sortOrder = "asc",
        int pageNumber = 1,
        int pageSize = 5)
    {
        // Bouw het ViewModel op met alle gefilterde, gesorteerde en gepagineerde boeken
        var vm = await BuildBoekenListViewModelAsync(q, catId, sortBy, sortOrder, pageNumber, pageSize);

        // Vul de dropdown lijst van categorieën voor de zoek/filter balk op de pagina
        ViewBag.Cats = new SelectList(await _db.Categorieen.Where(c => !c.IsDeleted).OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam", catId);

        // Stuur het ViewModel naar de Razor View
        return View(vm);
    }

    // GET: /Boeken/IndexPartial
    // AJAX-endpoint: Geeft alleen de partial view (_BoekenTablePartial.cshtml) terug bij live zoeken of pagina wisselen via Javascript
    [HttpGet]
    public async Task<IActionResult> IndexPartial(
        string? q = null,
        int? catId = null,
        string sortBy = "Titel",
        string sortOrder = "asc",
        int pageNumber = 1,
        int pageSize = 5)
    {
        var vm = await BuildBoekenListViewModelAsync(q, catId, sortBy, sortOrder, pageNumber, pageSize);
        return PartialView("_BoekenTablePartial", vm);
    }

    // POST: /Boeken/QuickToggleAvailability/5
    // AJAX-endpoint: Schakelt de beschikbaarheid van een boek direct om zonder dat de pagina hoeft te herladen
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> QuickToggleAvailability(int id)
    {
        // Zoek het boek op in de databank op basis van het unieke ID
        var boek = await _db.Boeken.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (boek == null) return NotFound(new { success = false, message = "Boek niet gevonden" });

        // Keer de status om (als het True is wordt het False, en omgekeerd)
        boek.IsBeschikbaar = !boek.IsBeschikbaar;

        // Sla de wijziging asynchroon op in de database
        await _db.SaveChangesAsync();

        // Geef een JSON resultaat terug aan de Javascript op de webpagina
        return Json(new { success = true, isBeschikbaar = boek.IsBeschikbaar, id = boek.Id });
    }

    // GET: /Boeken/Create
    // Alleen Admin en Medewerker mogen een nieuw boek toevoegen
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Cats = new SelectList(await _db.Categorieen.Where(c => !c.IsDeleted).OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam");
        return View(new BoekEditVm());
    }

    // POST: /Boeken/Create
    // Verwerkt het ingevulde formulier om een nieuw boek in te voeren
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Create(BoekEditVm vm)
    {
        // Controleer of het ingevulde formulier voldoet aan de data-annotaties (validatie regels)
        if (!ModelState.IsValid)
        {
            ViewBag.Cats = new SelectList(await _db.Categorieen.Where(c => !c.IsDeleted).OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam", vm.CategorieId);
            return View(vm);
        }

        // Voeg het nieuwe boek object toe aan de DbSet
        _db.Boeken.Add(new Boek
        {
            Titel = vm.Titel,
            Auteur = vm.Auteur,
            ISBN = vm.ISBN,
            CategorieId = vm.CategorieId,
            IsBeschikbaar = vm.IsBeschikbaar
        });

        // Sla het nieuwe record op in de database
        await _db.SaveChangesAsync();

        // Toon een groen succesbericht op de indexpagina via TempData
        TempData["Success"] = "Boek succesvol toegevoegd.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Boeken/Edit/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var boek = await _db.Boeken.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (boek == null) return NotFound();

        ViewBag.Cats = new SelectList(await _db.Categorieen.Where(c => !c.IsDeleted).OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam", boek.CategorieId);

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

    // POST: /Boeken/Edit/5
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Edit(BoekEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cats = new SelectList(await _db.Categorieen.Where(c => !c.IsDeleted).OrderBy(c => c.Naam).ToListAsync(), "Id", "Naam", vm.CategorieId);
            return View(vm);
        }

        var boek = await _db.Boeken.FirstOrDefaultAsync(b => b.Id == vm.Id && !b.IsDeleted);
        if (boek == null) return NotFound();

        // Pas de velden aan van het bestaande databank-entiteit
        boek.Titel = vm.Titel;
        boek.Auteur = vm.Auteur;
        boek.ISBN = vm.ISBN;
        boek.CategorieId = vm.CategorieId;
        boek.IsBeschikbaar = vm.IsBeschikbaar;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Boek succesvol gewijzigd.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Boeken/Delete/5
    // Soft-delete implementatie: Het record wordt niet fysiek gewist, maar als verwijderd gemarkeerd (IsDeleted = true)
    [Authorize(Roles = "Admin,Medewerker")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var boek = await _db.Boeken.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (boek == null) return NotFound();

        // Markeer als verwijderd en sla de datum/tijd op
        boek.IsDeleted = true;
        boek.DeletedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Boek succesvol verwijderd.";
        return RedirectToAction(nameof(Index));
    }

    // Helper methode: Bouwt de gefilterde, gesorteerde en gepagineerde lijst op
    private async Task<BoekenListViewModel> BuildBoekenListViewModelAsync(
        string? q, int? catId, string sortBy, string sortOrder, int pageNumber, int pageSize)
    {
        // 1. Begin met de basis-query op alle niet-verwijderde boeken met hun categorie
        var query = _db.Boeken.Include(b => b.Categorie).Where(b => !b.IsDeleted).AsQueryable();

        // 2. Filter op Categorie als er een categorieId is meegegeven
        if (catId.HasValue)
            query = query.Where(b => b.CategorieId == catId.Value);

        // 3. Filter op zoektekst (titel, auteur of ISBN) als de gebruiker iets intypt
        if (!string.IsNullOrWhiteSpace(q))
        {
            var t = q.Trim().ToLower();
            query = query.Where(b =>
                b.Titel.ToLower().Contains(t) ||
                b.Auteur.ToLower().Contains(t) ||
                b.ISBN.ToLower().Contains(t));
        }

        // 4. Bepaal de sortering (oplopend 'asc' of aflopend 'desc')
        bool isDesc = sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy.ToLower() switch
        {
            "auteur" => isDesc ? query.OrderByDescending(b => b.Auteur) : query.OrderBy(b => b.Auteur),
            "isbn" => isDesc ? query.OrderByDescending(b => b.ISBN) : query.OrderBy(b => b.ISBN),
            "categorie" => isDesc ? query.OrderByDescending(b => b.Categorie!.Naam) : query.OrderBy(b => b.Categorie!.Naam),
            "status" => isDesc ? query.OrderByDescending(b => b.IsBeschikbaar) : query.OrderBy(b => b.IsBeschikbaar),
            _ => isDesc ? query.OrderByDescending(b => b.Titel) : query.OrderBy(b => b.Titel)
        };

        // 5. Bereken paginering (aantal elementen, totaal pagina's, en neem alleen de rijen voor de gekozen pagina)
        int totalItems = await query.CountAsync();
        int safePageSize = pageSize < 1 ? 5 : pageSize;
        int totalPages = (int)Math.Ceiling(totalItems / (double)safePageSize);
        if (totalPages < 1) totalPages = 1;

        int safePageNumber = Math.Clamp(pageNumber, 1, totalPages);

        // Skip slaat de vorige pagina's over, Take neemt precies het aantal items voor deze pagina
        var boeken = await query
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync();

        // Geeft het kant-en-klare ViewModel terug
        return new BoekenListViewModel
        {
            Boeken = boeken,
            SearchQuery = q,
            CategorieId = catId,
            SortBy = sortBy,
            SortOrder = sortOrder,
            CurrentPage = safePageNumber,
            PageSize = safePageSize,
            TotalPages = totalPages,
            TotalItems = totalItems
        };
    }
}
