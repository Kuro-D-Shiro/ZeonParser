using AngleSharp.Dom;
using ZeonService.Models;
using ZeonService.Parser.Extentions;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    public class ZeonProductParser(IDownloadAndSaveImageService downloadAndSaveImageService,
        IProductRepository productRepository,
        ILogger<ZeonProductParser> logger) : IZeonProductParser
    {
        private readonly IDownloadAndSaveImageService downloadAndSaveImageService = downloadAndSaveImageService;
        private readonly IProductRepository productRepository = productRepository;
        private readonly ILogger<ZeonProductParser> logger = logger;

        public async Task<(Product?, bool)> Parse(IElement productElement, long? categoryId)
        {
            bool alreadyExists = true;

            var name = GetProductName(productElement);
            if (name == null)
            {
                logger.LogError("Не удалось получить имя товара со страницы.");
                return (null, alreadyExists);
            }

            var link = GetProductLink(productElement);
            if (link == null)
            {
                logger.LogError("Не удалось получить ссылку на товар со страницы.");
                return (null, alreadyExists);
            }

            string? imagePath = null;
            if (!await productRepository.IsExists(link))
            {
                imagePath = await GetProductImagePath(productElement);
                alreadyExists = false;
            }

            var priceElement = productElement?.QuerySelector("div.item-panel-price2-block")
                ?? null;
            if (priceElement == null)
            {
                logger.LogError("Не удалось получить информацию о ценах товара со страницы.");
                return (null, alreadyExists);
            }

            var currentPrice = GetCurrentProductPrice(productElement);
            if (currentPrice == null)
            {
                logger.LogError("Не удалось получить текущую цену товара со страницы.");
                return (null, alreadyExists);
            }

            var oldPrice = GetOldProductPrice(productElement);
            var specifications = GetProductSpecifications(productElement);
            if (categoryId == null)
            {
                logger.LogError("У товара не может отсутствовать идентификатор категории, которой он принадлежит.");
                return (null, alreadyExists);
            }

            return (new Product
            {
                Name = name,
                Link = link,
                ImagePath = imagePath,
                CurrentPrice = currentPrice.Value,
                OldPrice = oldPrice,
                Specifications = specifications,
                CategoryId = categoryId.Value
            }, alreadyExists);
        }

        private string? GetProductName(IElement productElement)
        {
            return productElement?.QuerySelector("h1.item-showcase-caption")?.TextContent.Trim()
                ?? null;
        }

        private string? GetProductLink(IElement productElement)
        {
            return productElement.QuerySelector("meta[property=\"og:url\"]")?.GetAttribute("content")
                ?? null;
        }

        private async Task<string> GetProductImagePath(IElement productElement)
        {
            var imageResult = await downloadAndSaveImageService.DownloadAndSaveImage(
                    productElement.QuerySelector("a.item-gallery-current")?.GetAttribute("href")
                    ?? throw new Exception("Ссылка на картинку не нашлась."),
                    Guid.NewGuid());

            if (imageResult.IsFailed)
                throw new Exception(string.Join(", ", imageResult.Reasons.Select(r => r.Message)));
            else
                return imageResult.Value;
        }

        private Dictionary<string, Dictionary<string, string>>?
            GetProductSpecifications(IElement productElement)
        {
            return productElement.QuerySelector("div.tabcontrol-content-inner table.table-params")?
                .ParseSpecificationFromHtmlTable()
                ?? null;
        }

        private decimal? GetCurrentProductPrice(IElement priceElement)
        {
            decimal? currentPrice = priceElement.QuerySelector("span.item-panel-price2-cur")?.TextContent
                .ParsePriceFromString()
                ?? null;
            if (currentPrice == null)
                return null;
            return currentPrice;
        }

        private decimal? GetOldProductPrice(IElement priceElement)
        {
            string oldPriceString = priceElement.QuerySelector("span.item-panel-price2-old")?.TextContent;
            return String.IsNullOrEmpty(oldPriceString) ? null : oldPriceString.ParsePriceFromString();
        }
    }
}
