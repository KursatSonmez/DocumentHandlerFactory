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

		public bool FileExists(string path, CancellationToken cancellationToken = default)
			=> File.Exists(GetPath(path));

		public async Task<bool> DirectoryExistsAsync(string directory, CancellationToken cancellationToken = default)
			=> await Task.Run(() => Directory.Exists(GetPath(directory)), cancellationToken);

		public bool DirectoryExists(string directory, CancellationToken cancellationToken = default)
			=> Directory.Exists(GetPath(directory));

		#endregion


		#region CreateDirectory

		public async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
			=> await Task.Run(() => Directory.CreateDirectory(GetPath(path)), cancellationToken);

		public void CreateDirectory(string path, CancellationToken cancellationToken = default)
			=> Directory.CreateDirectory(GetPath(path));

		#endregion

		#region DeleteDirectory

		public async Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
			=> await Task.Run(() => Directory.Delete(path), cancellationToken);

		public void DeleteDirectory(string path, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Directory.Delete(path);
		}

		#endregion


		#region ReadFile

		public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
			=> await File.ReadAllBytesAsync(GetPath(path), cancellationToken);

		public byte[] ReadAllBytes(string path, CancellationToken cancellationToken = default)
			=> File.ReadAllBytes(GetPath(path));

		public async Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default)
			=> Convert.ToBase64String(await ReadAllBytesAsync(path, cancellationToken));

		public string ReadAsBase64(string path, CancellationToken cancellationToken = default)
			=> Convert.ToBase64String(ReadAllBytes(path, cancellationToken));

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

		public string ReadAllText(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			string result;

			using (StreamReader reader = new(GetPath(path), encoding))
				result = reader.ReadToEnd();

			return result;
		}

		#endregion


		#region UploadFile

		public async Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default)
			=> await File.WriteAllBytesAsync(GetPath(path), buffer, cancellationToken);

		public void UploadFile(string path, byte[] buffer, CancellationToken cancellationToken = default)
			=> File.WriteAllBytes(GetPath(path), buffer);

		#endregion


		#region DeleteFile

		public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
		{
			var getPath = GetPath(path);

			if (File.Exists(getPath))
				await Task.Run(() => File.Delete(getPath), cancellationToken);
		}

		public void DeleteFile(string path, CancellationToken cancellationToken = default)
		{
			var getPath = GetPath(path);

			if (File.Exists(getPath))
				File.Delete(getPath);
		}

		#endregion


		#region CopyFile

		public async Task CopyFileAsync(string sourceFilePath, string destinationPath, CancellationToken cancellationToken = default)
		{
			byte[] bytes = await ReadAllBytesAsync(sourceFilePath, cancellationToken);
			await UploadFileAsync(destinationPath, bytes, cancellationToken);
		}

		public void CopyFile(string sourceFilePath, string destinationPath, CancellationToken cancellationToken = default)
		{
			byte[] bytes = ReadAllBytes(sourceFilePath, cancellationToken);
			UploadFile(destinationPath, bytes, cancellationToken);
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
