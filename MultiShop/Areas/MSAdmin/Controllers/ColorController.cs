using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.MSAdmin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.MSAdmin.Controllers
{
    [Area("MSAdmin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var colors = await _context.Colors.Where(c=>c.IgnoreQuery==false).Include(c => c.ProductColors).ToListAsync();
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ColorCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Colors.Any(c => c.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This Color is already exist");
                return View();
            }
            Color color = new()
            {
                Name = vm.Name
            };
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (color is null) return NotFound();

            ColorUpdateVM vm = new() { Name = color.Name };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ColorUpdateVM colorVM)
        {
            if (!ModelState.IsValid) return View();

            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            bool result = _context.Colors.Any(c => c.Name == colorVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This Color is already exist");
                return View();
            }

            existed.Name = colorVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            _context.Colors.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var color = await _context.Colors
                .Include(c => c.ProductColors)
                .ThenInclude(pc => pc.Product)
                .ThenInclude(p => p.ProductImages).
                FirstOrDefaultAsync(s => s.Id == id);
            if (color == null) return NotFound();

            return View(color);
        }
    }
}
