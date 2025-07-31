namespace ZeonParser.Models
{
    public class Category
    {
        public int CategoryId { get; set; } 
        public string Name { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
