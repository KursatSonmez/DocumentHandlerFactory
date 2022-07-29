using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Services.Interfaces
{
    /// <summary>
    /// All operations are performed under the temp folder.
    /// </summary>
    public interface ITempFileHandlerService: IDisposable
    {
        Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default);

        Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);

        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

        Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default);

        Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default);
    }
}
