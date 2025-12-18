using Microsoft.AspNetCore.Mvc;

namespace MijnBibliotheekWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
