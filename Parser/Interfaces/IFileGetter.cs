using FluentResults;

namespace ZeonService.Parser.Interfaces
{
    public interface IFileGetter<T1, T2>
    {
        Task<Result<T2>> Get(T1 selector);
    }
}
