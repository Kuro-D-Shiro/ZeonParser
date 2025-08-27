using FluentResults;
using ZeonService.Models;

namespace ZeonService.Parser.Interfaces
{
    public interface IZeonProductParser : IZeonElementParser<(Product?, bool)>
    {
    }
}
