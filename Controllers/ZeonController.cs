using Microsoft.AspNetCore.Mvc;
using ZeonService.Parser.Interfaces;
using ZeonService.ZeonParserDTO;

namespace ZeonService.Controllers
{
    [ApiController]
    [Route("api/parser/[controller]")]
    public class ZeonController(IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IFileGetter<Guid, (byte[], string)> imageGetter) : ControllerBase
    {
        [HttpGet("products/{productName:required}")]
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

        [HttpGet("productImage/{imageId:guid:required}")]
        public async Task<ActionResult> GetProductImage([FromRoute]Guid imageId)
        {
            var result = await imageGetter.Get(imageId);

            if (result.IsFailed)
                return StatusCode(418, result.Reasons);

            var (imageBytes, ext) = result.Value;
            return File(imageBytes, $"image/{ext}");
        }

        [HttpGet("products/{categoryId:long:required}")]
        public async Task<ActionResult<ProductWithoutCategoryDTO[]>> GetProductsByCategoryId([FromRoute]long categoryId)
        {
            var products = await productRepository.GetAllByCategoryId(categoryId);
           
            if (!products.Any())
                return NotFound("У категории нет товаров");

            var tasksProductsWithoutCategoryDTO = products.Select(p => ProductWithoutCategoryDTO.Create(p));
            var productsWithoutCategoryDTO = await Task.WhenAll(tasksProductsWithoutCategoryDTO);

            return Ok(productsWithoutCategoryDTO);
        }

        [HttpGet("categories/{categoryId:long:required}")]
        public async Task<ActionResult<CategoryWithoutHierarchyDTO[]>> GetСhildCategoriesByCategoryId([FromRoute] long categoryId)
        {
            var categories = await categoryRepository.GetAllByCategoryId(categoryId);

            if (!categories.Any())
                return NotFound("У категории нет категорий");

            var tasksCategorieWithoutHierarchyDTO = categories.Select(c => CategoryWithoutHierarchyDTO.Create(c));
            var categorieWithoutHierarchyDTO = await Task.WhenAll(tasksCategorieWithoutHierarchyDTO);

            return Ok(categorieWithoutHierarchyDTO);
        }
    }
}
