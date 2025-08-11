using ZeonService.Models;

namespace ZeonService.Parser.Interfaces
{
    public interface IZeonElementParser<T> 
    {
        Task<T> Parse(Category category);
    }
}
