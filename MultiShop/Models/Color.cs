using MultiShop.Models.Base;

namespace MultiShop.Models
{
    public class Color:BaseNameableModel
    {
        public ICollection<ProductColor>? ProductColors { get; set; }
    }
}
