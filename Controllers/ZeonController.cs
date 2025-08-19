using Microsoft.AspNetCore.Mvc;
using System.IO;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;
using ZeonService.ZeonParserDTO;

namespace ZeonService.Controllers
{
    [ApiController]
    [Route("api/parser/[controller]/[action]")]
    public class ZeonController(IProductRepository productRepository) : ControllerBase
    {
        [HttpGet("{productName}")]
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

        [HttpGet("{imagePath}")]
        public async Task<ActionResult> GetProductImage([FromRoute]string imagePath)
        {
            var fullPath = $"ProductImages/{imagePath}";
            
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var ext = Path.GetExtension(imagePath).Replace(".", "");

            return File(imageBytes, $"image/{ext}");
        }
    }
}
