using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MSAdmin.ViewModels
{
    public class ColorUpdateVM
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
