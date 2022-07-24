using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Senders.Interfaces;
using DocumentHandlerFactory.Settings;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Senders
{
    public class FileSender : ISender
    {
        protected readonly DocumentHandlerSettings Settings;

        public FileSender(DocumentHandlerSettings settings)
        {
            settings.Validate();

            Settings = settings;
        }

        protected string BaseDirectory => Settings.FileSettings.BaseDirectory;


        #region Exists

        public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
            => await Task.Run(() => File.Exists(GetPath(path)), cancellationToken);

        public async Task<bool> DirectoryExistsAsync(string directory, CancellationToken cancellationToken = default)
            => await Task.Run(() => Directory.Exists(GetPath(directory)), cancellationToken);

        #endregion


        #region CreateDirectory

        public async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
            => await Task.Run(() => Directory.CreateDirectory(GetPath(path)), cancellationToken);

        #endregion


        #region ReadFile

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
            => await File.ReadAllBytesAsync(GetPath(path), cancellationToken);

        public async Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default)
            => Convert.ToBase64String(await ReadAllBytesAsync(path, cancellationToken));

        public async Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (encoding == null)
                encoding = Encoding.UTF8;

            string result;

            using (StreamReader reader = new(GetPath(path), encoding))
                result = await reader.ReadToEndAsync();

            return result;
        }

        #endregion


        #region UploadFile

        public async Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default)
            => await File.WriteAllBytesAsync(GetPath(path), buffer, cancellationToken);

        #endregion


        #region DeleteFile

        public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
        {
            var getPath = GetPath(path);

            if (File.Exists(getPath))
                await Task.Run(() => File.Delete(getPath), cancellationToken);
        }

        #endregion


        #region CopyFile

        public async Task CopyFileAsync(string sourceFilePath, string destinationPath, CancellationToken cancellationToken = default)
        {
            byte[] bytes = await ReadAllBytesAsync(sourceFilePath, cancellationToken);
            await UploadFileAsync(destinationPath, bytes, cancellationToken);
        }

        #endregion



        #region Helpers
        protected string GetPath(string path) => BetterPath.Combine(BaseDirectory, path);
        #endregion

        #region IDisposable
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Dispose Managed Resources
            }

            // Dispose Native Resources

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileSender()
        {
            Dispose(false);
        }
        #endregion
    }
}
