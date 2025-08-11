using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ImageToFileSaver : IImageSaver
    {
        public async Task Save(byte[] data, string path)
        {
            await File.WriteAllBytesAsync(path, data);
        }
    }
}
