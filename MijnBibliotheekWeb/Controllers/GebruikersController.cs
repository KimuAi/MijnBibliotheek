using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Identity;

namespace MijnBibliotheekWeb.Controllers;

[Authorize(Roles = "Admin")]
public class GebruikersController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public GebruikersController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBlock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Admin mag zichzelf niet blokkeren
        if (User.Identity?.Name == user.UserName)
        {
            TempData["Error"] = "Je kan jezelf niet blokkeren.";
            return RedirectToAction(nameof(Index));
        }

        user.IsGeblokkeerd = !user.IsGeblokkeerd;
        await _userManager.UpdateAsync(user);

        // Als de gebruiker geblokkeerd is, update de security stamp (forceert her-login)
        if (user.IsGeblokkeerd)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        return RedirectToAction(nameof(Index));
    }
}
