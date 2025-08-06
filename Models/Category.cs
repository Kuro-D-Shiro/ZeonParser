namespace ZeonService.Models
{
    public class Category
    {
        public int CategoryId { get; set; } 
        public string Name { get; set; }
        public string Link { get; set; }
        public long? ParentCategoryId { get; set; } = null;
        public Category? ParentCategory { get; set; } = null;
        public ICollection<Category>? ChildCategories { get; set; } = null;
        public ICollection<Product> Products { get; set; } = [];
    }
}
