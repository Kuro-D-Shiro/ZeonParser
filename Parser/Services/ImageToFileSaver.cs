using FluentResults;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ImageToFileSaver : IImageSaver
    {
        public async Task<Result<string>> Save(byte[] data, string path)
        {
            try
            {
                await File.WriteAllBytesAsync(path, data);
                return Result.Ok(path);
            }
            catch(Exception ex) 
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
