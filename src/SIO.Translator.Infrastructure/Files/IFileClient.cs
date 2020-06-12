using System.IO;
using System.Threading.Tasks;

namespace SIO.Translator.Infrastructure.Files
{
    public interface IFileClient
    {
        Task UploadAsync(string fileName, string userId, Stream stream);
        Task<FileResult> DownloadAsync(string fileName, string userId);
    }
}
