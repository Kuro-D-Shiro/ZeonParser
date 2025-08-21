namespace ZeonService.Models
{
    public class Product
    {
        public long ProductId { get; set; } 
        public string Name { get; set; }
        public string Link { get; set; }
        public string ImagePath { get; set; }
        public Decimal? OldPrice { get; set; }
        public Decimal CurrentPrice { get; set; }
        public Dictionary<string, Dictionary<string, string>>? Specifications { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; } = new();
    }
}
