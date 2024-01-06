using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MSAdmin.ViewModels
{
    public class SlideUpdateVM
    {
        [Required]
        [MaxLength(50,ErrorMessage = "Max length is 50")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
