using Microsoft.AspNetCore.Mvc;
using ZeonService.Models;
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
        [HttpGet("products/byName/{productName:required}")]
        public async Task<ActionResult<ProductWithCategoryDTO[]>> GetProductsByName([FromRoute] string productName)
        {
            throw new Exception("ГОЙДА");
            var products = await productRepository.GetAllByName(productName);

            if (!products.Any())
                return NotFound($"Товаров с именем '{productName}' не найдено.");

            var productsWithCategoryDTO = products.Select(p => ProductWithCategoryDTO.Create(p));

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

        [HttpGet("products/byCategoryId/{categoryId:long:required}")]
        public async Task<ActionResult<ProductWithoutCategoryDTO[]>> GetProductsByCategoryId([FromRoute]long categoryId)
        {
            var products = await productRepository.GetAllByCategoryId(categoryId);
           
            if (!products.Any())
                return NotFound("У категории нет товаров");

            var productsWithoutCategoryDTO = products.Select(p => ProductWithoutCategoryDTO.Create(p));

            return Ok(productsWithoutCategoryDTO);
        }

        [HttpGet("categories/byParentCategoryId/{categoryId:long:required}")]
        public async Task<ActionResult<CategoryWithoutHierarchyDTO[]>> GetСhildCategoriesByCategoryId([FromRoute] long categoryId)
        {
            var categories = await categoryRepository.GetAllByCategoryId(categoryId);

            if (!categories.Any())
                return NotFound("У категории нет категорий");

            var categorieWithoutHierarchyDTO = categories.Select(c => CategoryWithoutHierarchyDTO.Create(c));

            return Ok(categorieWithoutHierarchyDTO);
        }

        [HttpGet("categories/main")]
        public async Task<ActionResult<CategoryWithoutHierarchyDTO[]>> GetMainCategories()
        {
            var mainCategories = await categoryRepository.GetMainCategories();

            if (!mainCategories.Any())
                return NotFound("Не нашлось основных категорий");

            var categorieWithoutHierarchyDTO = mainCategories.Select(c => CategoryWithoutHierarchyDTO.Create(c));

            return Ok(categorieWithoutHierarchyDTO);
        }

        [HttpGet("products/list")]
        public async Task<ActionResult<ProductWithCategoryDTO[]>> GetProductsList()
        {
            var products = await productRepository.GetAll();

            if (!products.Any())
                return NotFound("Товаров не найдено.");

            var productsWithCategoryDTO = products.Select(p => ProductWithCategoryDTO.Create(p));

            return Ok(productsWithCategoryDTO);
        }

        [HttpGet("product/byId/{id:long:required}")]
        public async Task<ActionResult<ProductWithCategoryDTO[]>> GetProductById([FromRoute] long id)
        {
            var product = await productRepository.GetById(id);

            if (product == null)
                return NotFound($"Товар c {id} не найдено.");

            var productsWithCategoryDTO = ProductWithCategoryDTO.Create(product);

            return Ok(productsWithCategoryDTO);
        }
    }
}
