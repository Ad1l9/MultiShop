using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModel;

namespace MultiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.Where(s=>s.IgnoreQuery==false).OrderBy(s=>s.Order).ToListAsync();
            List<Category> categories = await _context.Categories.Where(s => s.IgnoreQuery == false).Include(c => c.Products).ToListAsync();
            List<Product> products = await _context.Products.Where(s => s.IgnoreQuery == false).Include(c => c.Category).ToListAsync();

            HomeVM vm = new() { Slides = slides, Categories=categories, Products=products};
            return View(vm);
        }
    }
}
