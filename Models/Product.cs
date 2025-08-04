namespace ZeonParser.Models
{
    public class Product
    {
        public string Code { get; set; } 
        public string Name { get; set; } 
        public Image Image { get; set; }
        public Decimal? PriceWithoutDiscount { get; set; }
        public Decimal PriceWithDiscount { get; set; }
        public bool InStock { get; set; }
        public Category Category { get; set; } = new();
    }
    public class Image
    {
        public string Format { get; set; }
        public string Base64Content { get; set; }
    }
}
