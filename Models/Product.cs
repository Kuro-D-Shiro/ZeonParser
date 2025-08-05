namespace ZeonParser.Models
{
    public class Product
    {
        public long ProductId { get; set; } 
        public string Name { get; set; } 
        public Image Image { get; set; }
        public Decimal? OldPrice { get; set; }
        public Decimal CurrentPrice { get; set; }
        public bool InStock { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; } = new();
    }
    public class Image
    {
        public string Format { get; set; }
        public string Base64Content { get; set; }
    }
}
