using AngleSharp.Dom;
using ZeonService.Models;
using ZeonService.Parser.Extentions;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    public class ZeonProductParser(IDownloadAndSaveImageService downloadAndSaveImageService) : IZeonProductParser
    {
        private readonly IDownloadAndSaveImageService downloadAndSaveImageService = downloadAndSaveImageService;

        public async Task<Product> Parse(IElement productElement, long? categoryId)
        {
            var product = new Product();

            IElement productInfo = productElement?.QuerySelector(".catalog-item-info")
                ?? throw new Exception(productElement.OuterHtml);
            IElement productPrice = productElement?.QuerySelector(".catalog-item-price-main")
                ?? throw new Exception(productElement.OuterHtml);

            product.Name = productInfo?.QuerySelector("a")?.TextContent.Trim()
                ?? throw new Exception("У элемента не было текстового контента");
            product.ImagePath = await downloadAndSaveImageService.DownloadAndSaveImage(
                productElement.QuerySelector("img")?.GetAttribute("src")
                ?? throw new Exception("Ссылка на картинку не нашлась"),
                product.Name)
                ?? throw new Exception("Картинка не нашлась");
            product.Link = productInfo.QuerySelector("a")?.GetAttribute("href")
                ?? throw new Exception("У элемента не было аттрибута href");
            product.CurrentPrice = productPrice.QuerySelector(".value")?.TextContent.ParsePriceFromString()
                ?? throw new Exception("Не нашёлся тег с ценой");
            product.OldPrice = productPrice.QuerySelector(".catalog-item-price-old")?.TextContent.ParsePriceFromString()
                ?? null;
            product.Description = productInfo.QuerySelector(".if-size-not-pc a")?.TextContent.Replace("[", "").Replace("]", "")
                ?? throw new Exception("Не нашёлся тег с описанием");
            product.CategoryId = categoryId
                ?? throw new Exception("Товар без категории быть не может");
            
            return product;
        }
    }
}
