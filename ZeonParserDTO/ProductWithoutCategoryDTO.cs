using ZeonService.Models;

namespace ZeonService.ZeonParserDTO
{
    public class ProductWithoutCategoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
        public Decimal? OldPrice { get; set; }
        public Decimal CurrentPrice { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static async Task<ProductWithoutCategoryDTO> Create(Product product)
        {
            var imageLink = $"/api/parser/Zeon/GetProductImage/{product.ImagePath.Split('.')[0]}";

            return new ProductWithoutCategoryDTO
            {
                Id = product.ProductId,
                Name = product.Name,
                Link = product.Link,
                Description = product.Description,
                ImageLink = imageLink,
                OldPrice = product.OldPrice,
                CurrentPrice = product.CurrentPrice,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
