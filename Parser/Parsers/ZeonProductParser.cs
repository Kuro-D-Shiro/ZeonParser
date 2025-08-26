using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using FluentResults;
using ZeonService.Models;
using ZeonService.Parser.Extentions;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    public class ZeonProductParser(IDownloadAndSaveImageService downloadAndSaveImageService,
        IProductRepository productRepository) : IZeonProductParser
    {
        private readonly IDownloadAndSaveImageService downloadAndSaveImageService = downloadAndSaveImageService;

        public async Task<Product> Parse(IElement productElement, long? categoryId)
        {
            var product = new Product();

            IElement productPrice = productElement?.QuerySelector("div.item-panel-price2-block")
                ?? throw new Exception("Не нашлось блока с ценой товара.");

            product.Name = productElement?.QuerySelector("h1.item-showcase-caption")?.TextContent.Trim()
                ?? throw new Exception("У элемента не было текстового контента.");
            product.Link = productElement.QuerySelector("meta[property=\"og:url\"]")?.GetAttribute("content")
                ?? throw new Exception("У элемента не было аттрибута content.");
            if (!await productRepository.IsExists(product.Link))
            {
                product.ImagePath = await downloadAndSaveImageService.DownloadAndSaveImage(
                    productElement.QuerySelector("a.item-gallery-current")?.GetAttribute("href")
                    ?? throw new Exception("Ссылка на картинку не нашлась."),
                    Guid.NewGuid())
                    ?? throw new Exception("Картинка не нашлась.");
            }
            product.CurrentPrice = productPrice.QuerySelector("span.item-panel-price2-cur")?.TextContent
                .ParsePriceFromString()
                ?? throw new Exception("Не нашёлся тег с ценой.");
            string oldPriceString = productPrice.QuerySelector("span.item-panel-price2-old")?.TextContent;
            product.OldPrice = String.IsNullOrEmpty(oldPriceString) ? null : oldPriceString.ParsePriceFromString();
            product.Specifications = productElement.QuerySelector("div.tabcontrol-content-inner table.table-params")?
                .ParseSpecificationFromHtmlTable()
                ?? null;
            product.CategoryId = categoryId
                ?? throw new Exception("Товар без категории быть не может");

            return product;
        }
    }
}
