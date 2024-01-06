using MultiShop.Models.Base;

namespace MultiShop.Models
{
    public class Category:BaseNameableModel
    {
        public ICollection<Product>? Products { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
