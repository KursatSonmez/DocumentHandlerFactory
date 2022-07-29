using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Services.Interfaces;
using DocumentHandlerFactory.Settings;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Services
{
    public class TempFileHandlerService : ITempFileHandlerService
    {
        protected readonly TempFileSettings Settings;

        private readonly IDocumentHandlerService _documentHandlerService;

        public TempFileHandlerService(TempFileSettings settings)
        {
            Settings = settings;
            _documentHandlerService = new DocumentHandlerService((DocumentHandlerSettings)settings);
        }

        public async Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default)
            => await _documentHandlerService.UploadFileAsync(CombineTempPath(path), buffer, cancellationToken);

        public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
            => await _documentHandlerService.DeleteFileAsync(CombineTempPath(path), cancellationToken);

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
            => await _documentHandlerService.ReadAllBytesAsync(CombineTempPath(path), cancellationToken);

        public async Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
            => await _documentHandlerService.ReadAllTextAsync(CombineTempPath(path), encoding, cancellationToken);

        public async Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default)
            => await _documentHandlerService.ReadAsBase64Async(CombineTempPath(path), cancellationToken);


        #region Helpers

        private string CombineTempPath(string path)
            => Settings.HandlerType == Models.HandlerType.File
            ? BetterPath.Combine(Settings.FileSettings.BaseDirectory, path)
            : path;

        #endregion


        #region IDisposable

        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _documentHandlerService.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
