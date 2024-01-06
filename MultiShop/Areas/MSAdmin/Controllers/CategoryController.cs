using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.MSAdmin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extension;

namespace MultiShop.Areas.MSAdmin.Controllers
{
    [Area("MSAdmin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Where(c => c.IgnoreQuery == false).ToListAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid) return View();

            if (!vm.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Wrong file type");
                return View();
            }

            if (!vm.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "Size shouldn't be more than 2MB");
            }

            string fileName = await vm.Photo.CreateFile(_env.WebRootPath, "assets", "img");


            Category category = new()
            {
                Name = vm.Name,
                ImageUrl = fileName
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) return NotFound();

            CategoryUpdateVM vm = new()
            {
                ImageUrl = category.ImageUrl,
                Name = category.Name
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, CategoryUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);


            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();


            bool result = _context.Categories.Any(c => c.Name == vm.Name && c.Id != id);
            if (!result)
            {
                if (vm.Photo is not null)
                {
                    if (!vm.Photo.ValidateType())
                    {
                        ModelState.AddModelError("Photo", "Wrong file type");
                        return View(vm);
                    }
                    if (!vm.Photo.ValidateSize(2 * 1024))
                    {
                        ModelState.AddModelError("Photo", "Size shouldn't be more than 2MB");
                        return View(vm);
                    }
                    string newImage = await vm.Photo.CreateFile(_env.WebRootPath, "assets", "img");
                    existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                    existed.ImageUrl = newImage;

                }


                existed.Name = vm.Name;
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("Name", "This Name is already exist");
                return View(existed);
            }


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePermanently(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            category.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) return NotFound();

            category.IgnoreQuery = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
