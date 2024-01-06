using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.MSAdmin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extension;

namespace MultiShop.Areas.MSAdmin.Controllers
{
    [Area("MSAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Where(p => p.IgnoreQuery == false)
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .ToListAsync();

            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            ProductCreateVM vm = new()
            {
                Categories = await _context.Categories.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                return View(vm);
            }

            bool result = await _context.Categories.AnyAsync(c => c.Id == vm.CategoryId);

            if (!result)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                vm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("CategoryId", "There is no such category");
                return View(vm);
            }

            foreach (int id in vm.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == id);
                if (!colorResult)
                {
                    vm.Categories = await _context.Categories.ToListAsync();
                    vm.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds", "There is no such color");
                    return View(vm);
                }
            }

            if (!vm.MainPhoto.ValidateType())
            {
                vm.Categories = await _context.Categories.ToListAsync();
                
                vm.Colors = await _context.Colors.ToListAsync();
                
                ModelState.AddModelError("MainPhoto", "Wrong file type");
                return View(vm);
            }
            if (!vm.MainPhoto.ValidateSize(600))
            {
                vm.Categories = await _context.Categories.ToListAsync();
                
                vm.Colors = await _context.Colors.ToListAsync();
                
                ModelState.AddModelError("MainPhoto", "Wrong file size");
                return View(vm);
            }
            if (!vm.HoverPhoto.ValidateType())
            {
                vm.Categories = await _context.Categories.ToListAsync();
                
                vm.Colors = await _context.Colors.ToListAsync();
                
                ModelState.AddModelError("HoverPhoto", "Wrong file type");
                return View(vm);
            }
            if (!vm.HoverPhoto.ValidateSize(600))
            {
                vm.Categories = await _context.Categories.ToListAsync();
                
                vm.Colors = await _context.Colors.ToListAsync();
                
                ModelState.AddModelError("HoverPhoto", "Wrong file size");
                return View(vm);
            }
            ProductImage image = new ProductImage
            {
                Alternative = vm.Name,
                IsPrimary = true,
                ImageURL = await vm.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img")
            };
            ProductImage hoverImage = new ProductImage
            {
                Alternative = vm.Name,
                IsPrimary = false,
                ImageURL = await vm.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img")
            };


            Product product = new Product
            {
                Name = vm.Name,
                Price = vm.Price,
                CategoryId = (int)vm.CategoryId,
                Description = vm.Description,
                ProductColors = new(),
                ProductImages = new()
                {
                    image,hoverImage
                }
            };

            foreach (int id in vm.ColorIds)
            {
                var pColor = new ProductColor
                {
                    ColorId = id,
                    Product = product
                };
                product.ProductColors.Add(pColor);
            }

            TempData["Message"] = "";
            foreach (IFormFile photo in vm.Photos)
            {
                if (!photo.ValidateType())
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
                    continue;
                }
                if (!photo.ValidateSize(600))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
                    continue;
                }

                product.ProductImages.Add(new ProductImage
                {
                    Alternative = product.Name,
                    IsPrimary = null,
                    ImageURL = await photo.CreateFile(_env.WebRootPath, "assets", "img")
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var existed = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            foreach (ProductImage image in existed.ProductImages)
            {
                image.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
            }
            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            var vm = new ProductUpdateVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ProductImages = product.ProductImages,
                ColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductUpdateVM productVM)
        {
            Product existed = await _context.Products
                .Include(p => p.ProductColors)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == id);
            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }


            if (existed is null) return NotFound();

            bool result = await _context.Products.AnyAsync(c => c.CategoryId == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }

            foreach (int idC in productVM.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == idC);
                if (!colorResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds", "There is no such color");
                    return View(productVM);
                }
            }


            result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("Name", "There is already such product");
                return View(productVM);
            }

            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType())
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "File type is not valid");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "File size is not valid");
                    return View(productVM);
                }
            }
            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.ValidateType())
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("HoverPhoto", "File type is not valid");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.ValidateSize(600))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("HoverPhoto", "File size is not valid");
                    return View(productVM);
                }
            }

            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "assets", "img");
                ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
                _context.ProductImages.Remove(mainImage);
                existed.ProductImages.Add(new ProductImage
                {
                    Alternative = productVM.Name,
                    IsPrimary = true,
                    ImageURL = fileName
                });
            }
            if (productVM.HoverPhoto is not null)
            {
                string fileName = await productVM.HoverPhoto.CreateFile(_env.WebRootPath, "assets", "img");
                ProductImage hoverImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                hoverImage.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
                _context.ProductImages.Remove(hoverImage);
                existed.ProductImages.Add(new ProductImage
                {
                    Alternative = productVM.Name,
                    IsPrimary = false,
                    ImageURL = fileName
                });
            }
            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new();
            }
            var removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage pi in removeable)
            {
                pi.ImageURL.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ProductImages.Remove(pi);
            }



            existed.ProductColors.RemoveAll(pColor => !productVM.ColorIds.Contains(pColor.Id));

            existed.ProductColors.AddRange(productVM.ColorIds.Where(colorId => !existed.ProductColors.Any(pt => pt.Id == colorId))
                                 .Select(colorId => new ProductColor { ColorId = colorId }));


            TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos)
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        Alternative = productVM.Name,
                        IsPrimary = null,
                        ImageURL = await photo.CreateFile(_env.WebRootPath, "assets", "img")
                    });
                }
            }

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.CategoryId = productVM.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
