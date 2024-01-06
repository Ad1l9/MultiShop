using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MultiShop.Areas.MSAdmin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extension;

namespace MultiShop.Areas.MSAdmin.Controllers
{
    [Area("MSAdmin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.Where(s=>s.IgnoreQuery==false).ToListAsync();
            return View(slides);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(SlideCreateVM vm)
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

            Slide exist = await _context.Slides.FirstOrDefaultAsync(s => s.Order == vm.Order);
            if (exist != null)
            {
                int newOrder = _context.Slides.OrderByDescending(s => s.Order).FirstOrDefault().Order;
                exist.Order = newOrder+1;
            }

            Slide slide = new()
            {
                Title = vm.Title,
                Description = vm.Description,
                ImageUrl = fileName,
                Order = vm.Order,
            };

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();

            SlideUpdateVM updateSlideVM = new()
            {
                ImageUrl = slide.ImageUrl,
                Title = slide.Title,
                Order = slide.Order,
                Description = slide.Description
            };

            return View(updateSlideVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, SlideUpdateVM updateSlideVM)
        {
            if (!ModelState.IsValid) return View(updateSlideVM);


            Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();


            bool result = _context.Slides.Any(s => s.Title == updateSlideVM.Title &&  s.Id != id);
            if (!result)
            {
                if (updateSlideVM.Photo is not null)
                {
                    if (!updateSlideVM.Photo.ValidateType())
                    {
                        ModelState.AddModelError("Photo", "Wrong file type");
                        return View(updateSlideVM);
                    }
                    if (!updateSlideVM.Photo.ValidateSize(2 * 1024))
                    {
                        ModelState.AddModelError("Photo", "Size shouldn't be more than 2MB");
                        return View(updateSlideVM);
                    }
                    string newImage = await updateSlideVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
                    existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                    existed.ImageUrl = newImage;

                }

                Slide existOrder = await _context.Slides.FirstOrDefaultAsync(s => s.Order == updateSlideVM.Order);
                if (existOrder != null)
                {
                    existOrder.Order=existed.Order;
                }

                existed.Title = updateSlideVM.Title;
                existed.Description = updateSlideVM.Description;
                existed.Order = updateSlideVM.Order;
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("Title", "This Title is already exist");
                return View(existed);
            }


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePermanently(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();

            slide.IgnoreQuery = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
