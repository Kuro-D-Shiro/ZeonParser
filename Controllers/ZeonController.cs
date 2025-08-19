using Microsoft.AspNetCore.Mvc;
using ZeonService.Parser.Interfaces;
using ZeonService.ZeonParserDTO;

namespace ZeonService.Controllers
{
    [ApiController]
    [Route("api/parser/[controller]/[action]")]
    public class ZeonController(IProductRepository productRepository,
        IFileGetter<Guid, (byte[], string)> imageGetter) : ControllerBase
    {
        [HttpGet("{productName:required}")]
        public async Task<ActionResult<ProductWithCategoryDTO[]>> GetProductsByName([FromRoute] string productName)
        {
            var products = await productRepository.GetAllByName(productName);

            if (!products.Any())
            {
                return NotFound($"Товаров с именем '{productName}' не найдено.");
            }

            var tasksProductsWithCategoryDTO = products.Select(p => ProductWithCategoryDTO.Create(p));

            var productsWithCategoryDTO = await Task.WhenAll(tasksProductsWithCategoryDTO);

            return Ok(productsWithCategoryDTO);
        }

        [HttpGet("{imagePath:guid:required}")]
        public async Task<ActionResult> GetProductImage([FromRoute]Guid imagePath)
        {
            var result = await imageGetter.Get(imagePath);

            if (result.IsFailed)
                return StatusCode(418, result.Reasons);

            var (imageBytes, ext) = result.Value;
            return File(imageBytes, $"image/{ext}");
        }

        [HttpGet("{categoryId:long:required}")]
        public async Task<ActionResult<ProductWithoutCategoryDTO[]>> GetProductsByCategoryId([FromRoute]long categoryId)
        {
            var products = await productRepository.GetAllByCategoryId(categoryId);
           
            if (!products.Any())
                return NotFound("У категории нет товаров");

            var tasksProductsWithoutCategoryDTO = products.Select(p => ProductWithoutCategoryDTO.Create(p));
            var productsWithoutCategoryDTO = await Task.WhenAll(tasksProductsWithoutCategoryDTO);

            return Ok(productsWithoutCategoryDTO);
        }
    }
}
