using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MSAdmin.ViewModels
{
    public class ColorCreateVM
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
