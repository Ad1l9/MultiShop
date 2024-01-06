using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MSAdmin.ViewModels
{
    public class CategoryCreateVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IFormFile Photo { get; set; } = null!;
    }
}
