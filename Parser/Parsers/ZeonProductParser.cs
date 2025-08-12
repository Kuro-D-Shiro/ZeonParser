using AngleSharp.Dom;
using ZeonService.Models;
using ZeonService.Parser.Extentions;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    public class ZeonProductParser(IDownloadAndSaveImageService downloadAndSaveImageService) : IZeonProductParser
    {
        private readonly IDownloadAndSaveImageService downloadAndSaveImageService = downloadAndSaveImageService;

        public async Task<Product> Parse(IElement productElement, Category category)
        {
            var product = new Product();

            IElement productInfo = productElement?.QuerySelector(".catalog-item-info")
                ?? throw new Exception("Лееееееееее инфы нет");
            IElement productPrice = productElement?.QuerySelector(".catalog-item-price-main")
                ?? throw new Exception("Лееееееееее цены нет");

            product.Name = productInfo?.TextContent
                ?? throw new Exception("У элемента не было текстового контента");
            product.ImagePath = await downloadAndSaveImageService.DownloadAndSaveImage(
                productElement.QuerySelector("img")?.GetAttribute("src")
                ?? throw new Exception("Ссылка на картинку не нашлась"),
                product.Name)
                ?? throw new Exception("Картинка не нашлась");
            product.Link = productInfo.GetAttribute("href")
                ?? throw new Exception("У элемента не было аттрибута href");
            product.CurrentPrice = productPrice.QuerySelector(".value")?.TextContent.ParsePriceFromString()
                ?? throw new Exception("Не нашёлся тег с ценой");
            product.OldPrice = productPrice.QuerySelector(".catalog-item-price-old")?.TextContent.ParsePriceFromString()
                ?? null;
            product.Category = category;
            
            return product;
        }
    }
}
