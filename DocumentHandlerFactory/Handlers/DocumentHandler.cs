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

		public virtual bool FileExists(string path, CancellationToken cancellationToken = default)
			=> Sender.FileExists(path, cancellationToken);

		public virtual async Task<bool> DirectoryExistsAsync(string directory, CancellationToken cancellationToken = default)
			=> await Sender.DirectoryExistsAsync(directory, cancellationToken);

		public virtual bool DirectoryExists(string directory, CancellationToken cancellationToken = default)
			=> Sender.DirectoryExists(directory, cancellationToken);

		#endregion


		#region CreateDirectory

		public virtual async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
			=> await Sender.CreateDirectoryAsync(path, cancellationToken);

		public virtual void CreateDirectory(string path, CancellationToken cancellationToken = default)
			=> Sender.CreateDirectory(path, cancellationToken);

		#endregion


		#region ReadFile

		public virtual async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
			=> await Sender.ReadAllBytesAsync(path, cancellationToken);

		public virtual byte[] ReadAllBytes(string path, CancellationToken cancellationToken = default)
			=> Sender.ReadAllBytes(path, cancellationToken);

		public virtual async Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default)
			=> await Sender.ReadAsBase64Async(path, cancellationToken);

		public virtual string ReadAsBase64(string path, CancellationToken cancellationToken = default)
			=> Sender.ReadAsBase64(path, cancellationToken);

		public async Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
			=> await Sender.ReadAllTextAsync(path, encoding, cancellationToken);

		public string ReadAllText(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
			=> Sender.ReadAllText(path, encoding, cancellationToken);

		#endregion


		#region UploadFile

		public virtual async Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default)
			=> await Sender.UploadFileAsync(path, buffer, cancellationToken);

		public virtual void UploadFile(string path, byte[] buffer, CancellationToken cancellationToken = default)
			=> Sender.UploadFile(path, buffer, cancellationToken);

		#endregion


		#region DeleteFile

		public virtual async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
			=> await Sender.DeleteFileAsync(path, cancellationToken);

		public virtual void DeleteFile(string path, CancellationToken cancellationToken = default)
			=> Sender.DeleteFile(path, cancellationToken);

		#endregion


		#region CopyFile

		public virtual async Task CopyFileAsync(string sourceFilePath, string destinationPath, CancellationToken cancellationToken = default)
			=> await Sender.CopyFileAsync(sourceFilePath, destinationPath, cancellationToken);

		public virtual void CopyFile(string sourceFilePath, string destinationPath, CancellationToken cancellationToken = default)
			=> Sender.CopyFile(sourceFilePath, destinationPath, cancellationToken);

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
