using Microsoft.AspNetCore.Mvc;

namespace MultiShop.Areas.MSAdmin.Controllers
{
    public class HomeController : Controller
    {
        [Area("MSAdmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
