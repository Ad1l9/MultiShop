namespace MultiShop.Models.Base
{
    public abstract class BaseNameableModel:BaseModel
    {
        public string Name { get; set; } = null!;
    }
}
