namespace ZeonService.Parser.Interfaces
{
    public interface IDataSaver<T>
    {
        Task Save(T data, string path);
    }
}
