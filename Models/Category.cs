namespace ZeonParser.Models
{
    public class Category
    {
        public int CategoryId { get; set; } 
        public string Name { get; set; }
        public string Link { get; set; }
        public Category? ParentCategory { get; set; }
    }
}
