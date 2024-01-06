using MultiShop.Models.Base;

namespace MultiShop.Models
{
    public class Product:BaseNameableModel
    {
        public decimal Price { get; set; }

        public decimal DiscountPrice { get; set; }

        public string Description { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
