namespace MultiShop.Models.Base
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public bool IgnoreQuery { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public BaseModel()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
