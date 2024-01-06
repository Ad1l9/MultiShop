using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MSAdmin.ViewModels
{
    public class SlideCreateVM
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }

        [Required]
        public IFormFile Photo { get; set; }    
    }
}
