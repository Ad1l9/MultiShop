using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MSAdmin.ViewModels
{
    public class CategoryUpdateVM
    {
        [Required]
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
