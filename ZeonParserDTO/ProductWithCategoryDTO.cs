using ZeonService.Models;

namespace ZeonService.ZeonParserDTO
{
    public class ProductWithCategoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
        public Decimal? OldPrice { get; set; }
        public Decimal CurrentPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryLink { get; set; }

        public static async Task<ProductWithCategoryDTO> Create(Product product)
        {
            var imageLink = $"/api/parser/Zeon/GetProductImage/{product.ImagePath.Split('.')[0]}";

            return new ProductWithCategoryDTO
            {
                Id = product.ProductId,
                Name = product.Name,
                Link = product.Link,
                Description = product.Description,
                ImageLink = imageLink,
                OldPrice = product.OldPrice,
                CurrentPrice = product.CurrentPrice,
                UpdatedAt = product.UpdatedAt,
                CategoryId = product.Category.CategoryId,
                CategoryName = product.Category.Name,
                CategoryLink = product.Category.Link
            };
        }
    }

    /*public class ProductWithCategoryDTO(Product product)
{
    public long Id { get; set; } = product.ProductId;
    public string Name { get; set; } = product.Name;
    public string Link { get; set; } = product.Link;
    public string Description { get; set; } = product.Description;
    public byte[] Image { get; set; } = File.ReadAllBytes(product.ImagePath);
    public Decimal? OldPrice { get; set; } = product.OldPrice;
    public Decimal CurrentPrice { get; set; } = product.CurrentPrice;
    public DateTime UpdatedAt { get; set; } = product.UpdatedAt;

    public long CategoryId { get; set; } = product.Category.CategoryId;
    public string CategoryName { get; set; } = product.Category.Name;
    public string CategoryLink { get; set; } = product.Category.Link;
}*/
}
