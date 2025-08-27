using ZeonService.Models;

namespace ZeonService.ZeonParserDTO
{
    public class ProductWithCategoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public Dictionary<string, Dictionary<string, string>> Specifications { get; set; }
        public string ImageLink { get; set; }
        public Decimal? OldPrice { get; set; }
        public Decimal CurrentPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryLink { get; set; }

        public static ProductWithCategoryDTO Create(Product product)
        {
            var imageLink = $"/api/parser/Zeon/GetProductImage/{product.ImagePath.Split('.')[0]}";

            return new ProductWithCategoryDTO
            {
                Id = product.ProductId,
                Name = product.Name,
                Link = product.Link,
                Specifications = product.Specifications,
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
}
