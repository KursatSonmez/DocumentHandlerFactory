using DocumentHandlerFactory.Handlers.Base;
using DocumentHandlerFactory.Handlers.Interfaces;
using DocumentHandlerFactory.Settings;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Handlers
{
    public class DocumentHandler : BaseDocumentHandler, IDocumentHandler
    {
        public DocumentHandler(DocumentHandlerSettings settings) : base(settings)
        {
        }


        #region Exists

        public virtual async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
            => await Sender.FileExistsAsync(path, cancellationToken);
        public virtual async Task<bool> DirectoryExistsAsync(string directory, CancellationToken cancellationToken = default)
            => await Sender.DirectoryExistsAsync(directory, cancellationToken);

        #endregion


        #region CreateDirectory

        public virtual async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
            => await Sender.CreateDirectoryAsync(path, cancellationToken);

        #endregion


        #region ReadFile

        public virtual async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
            => await Sender.ReadAllBytesAsync(path, cancellationToken);

        public virtual async Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default)
            => await Sender.ReadAsBase64Async(path, cancellationToken);

        public async Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
            => await Sender.ReadAllTextAsync(path, encoding, cancellationToken);

        #endregion


        #region UploadFile

        public virtual async Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default)
            => await Sender.UploadFileAsync(path, buffer, cancellationToken);

        #endregion


        #region DeleteFile

        public virtual async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
            => await Sender.DeleteFileAsync(path, cancellationToken);

        #endregion


        #region CopyFile

        public virtual async Task CopyFileAsync(string sourceFilePath, string destinationFolder, CancellationToken cancellationToken = default)
            => await Sender.CopyFileAsync(sourceFilePath, destinationFolder, cancellationToken);

        #endregion



        #region IDisposable
        protected override void DisposeManagedResources()
        {
        }

        protected override void DisposeNativeResources()
        {
        }

        ~DocumentHandler()
        {
            Dispose(false);
        }
        #endregion
    }
}
