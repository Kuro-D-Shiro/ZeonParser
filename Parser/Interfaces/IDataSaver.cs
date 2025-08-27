using FluentResults;

namespace ZeonService.Parser.Interfaces
{
    public interface IDataSaver<T>
    {
        Task<Result<string>> Save(T data, string path);
    }
}
