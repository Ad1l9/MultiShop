using MultiShop.Models.Base;

namespace MultiShop.Models
{
    public class Slide:BaseModel
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set;}
        public string ImageUrl { get; set; } = null!;
        public string ButtonText { get; set; } = "Shop Now";
        public int Order { get; set; }
        public bool IsActive { get; set; }

    }
}
