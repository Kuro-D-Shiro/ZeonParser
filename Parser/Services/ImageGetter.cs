using FluentResults;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ImageGetter : IFileGetter<Guid, (byte[], string)>
    {
        public async Task<Result<(byte[], string)>> Get(Guid selector)
        {
            var directoryPath = "ProductImages";

            try
            {
                var file = Directory.GetFiles(directoryPath)
                    .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f)
                        .Equals(selector.ToString(), StringComparison.OrdinalIgnoreCase));

                if (file == null)
                    return Result.Fail(new Error("Файл не найден."));

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(file);

                return Result.Ok((fileBytes, Path.GetExtension(file).Replace(".", "")));
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error(ex.Message));
            }
        }
    }
}
