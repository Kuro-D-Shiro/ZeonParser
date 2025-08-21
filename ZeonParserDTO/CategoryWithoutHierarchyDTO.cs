using ZeonService.Models;

namespace ZeonService.ZeonParserDTO
{
    public class CategoryWithoutHierarchyDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }

        public static async Task<CategoryWithoutHierarchyDTO> Create(Category category)
        {
            return new CategoryWithoutHierarchyDTO
            {
                Id = category.CategoryId,
                Name = category.Name,
                Link = category.Link
            };
        }
    }
}
