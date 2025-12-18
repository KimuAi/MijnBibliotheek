using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers
{
    public class AccountController : Controller
    {
        // /Account/Login  -> /Identity/Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var url = "/Identity/Account/Login";
            if (!string.IsNullOrWhiteSpace(returnUrl))
                url += $"?ReturnUrl={Uri.EscapeDataString(returnUrl)}";

            return Redirect(url);
        }

        // /Account/Logout -> /Identity/Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            return Redirect("/Identity/Account/Logout");
        }
    }
}
