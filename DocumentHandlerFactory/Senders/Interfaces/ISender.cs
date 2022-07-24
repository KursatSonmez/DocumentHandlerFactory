using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Senders.Interfaces
{
    public interface ISender : IDisposable
    {
        #region Exists
        Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);
        Task<bool> DirectoryExistsAsync(string directory, CancellationToken cancellationToken = default);
        #endregion


        #region CreateDirectory
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        #endregion


        #region ReadFile
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

        Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding">
        /// Default: <see cref="Encoding.UTF8"/>
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default);
        #endregion


        #region UploadFile
        Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default);
        #endregion


        #region DeleteFile
        Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
        #endregion


        #region CopyFile
        Task CopyFileAsync(string sourceFilePath, string destinationFolder, CancellationToken cancellationToken = default);
        #endregion
    }
}
