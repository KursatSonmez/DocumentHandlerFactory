using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Services.Interfaces;
using DocumentHandlerFactory.Settings;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Services
{
    public class TempFileHandlerService : ITempFileHandlerService
    {
        protected readonly TempFileSettings Settings;

        private readonly IDocumentHandlerService _documentHandlerService;

        public TempFileHandlerService(DocumentHandlerSettings settings, IDocumentHandlerService documentHandlerService)
        {
            Settings = settings.TempFileSettings;
            _documentHandlerService = documentHandlerService;
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
            => BetterPath.Combine(Settings.TempFolderFullPath, path);

        #endregion
    }
}
