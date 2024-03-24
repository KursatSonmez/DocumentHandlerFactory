using DocumentHandlerFactory.Senders.Interfaces;
using DocumentHandlerFactory.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentHandlerFactory.Senders
{
	public class FtpSender : ISender
	{
		protected readonly DocumentHandlerSettings Settings;

		public FtpSender(DocumentHandlerSettings settings)
		{
			settings.Validate();

			Settings = settings;
		}

		protected const string FileExistsMethod = WebRequestMethods.Ftp.GetFileSize;
		protected const string ForwardSlash = "/";

		protected string BaseUrl => Settings.FtpSettings.Url;
		protected string Username => Settings.FtpSettings.Username;
		protected string Password => Settings.FtpSettings.Password;


		private NetworkCredential _credential;
		protected NetworkCredential Credential
			=> _credential ??= new(Username, Password);

		private WebClient _webClient;
		protected WebClient WebClient
			=> _webClient ??= new WebClient() { Credentials = Credential };



		#region Exists

		public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
		{
			await ExecuteAsync(FileExistsMethod, path, cancellationToken);
			return true;
		}

		public bool FileExists(string path, CancellationToken cancellationToken = default)
		{
			Execute(FileExistsMethod, path, cancellationToken);
			return true;
		}

		public async Task<bool> DirectoryExistsAsync(string directory, CancellationToken cancellationToken = default)
		{
			// Important!! If we don't add 'ForwardSlash' to the end, no error occurs when file is not found!
			// https://stackoverflow.com/a/24047971/6393893
			// https://stackoverflow.com/questions/46052535/why-does-ftpwebrequest-return-an-empty-stream-for-this-existing-directory
			if (!directory.EndsWith(ForwardSlash))
				directory += ForwardSlash;

			try
			{
				FtpWebRequest req = GetRequest(directory, WebRequestMethods.Ftp.ListDirectory);
				using (FtpWebResponse response = (FtpWebResponse)await req.GetResponseAsync())
				{
					StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
					await sr.ReadToEndAsync();
					sr.Close();
					response.Close();
				}

				return true;
			}
			catch //(Exception e)
			{
				// ignored
			}

			return false;
		}

		public bool DirectoryExists(string directory, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// Important!! If we don't add 'ForwardSlash' to the end, no error occurs when file is not found!
			// https://stackoverflow.com/a/24047971/6393893
			// https://stackoverflow.com/questions/46052535/why-does-ftpwebrequest-return-an-empty-stream-for-this-existing-directory
			if (!directory.EndsWith(ForwardSlash))
				directory += ForwardSlash;

			try
			{
				FtpWebRequest req = GetRequest(directory, WebRequestMethods.Ftp.ListDirectory);
				using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
				{
					StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);
					sr.ReadToEnd();
					sr.Close();
					response.Close();
				}

				return true;
			}
			catch //(Exception e)
			{
				// ignored
			}

			return false;
		}

		#endregion


		#region CreateDirectory

		public async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
			=> await CreateFTPDirectory(path, cancellationToken);

		public void CreateDirectory(string path, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CreateFTPDirectory(path);
		}

		#endregion


		#region ReadFile

		public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
			=> await DownloadDataTaskAsync(GetUri(path), cancellationToken);

		public byte[] ReadAllBytes(string path, CancellationToken cancellationToken = default)
			=> DownloadData(GetUri(path), cancellationToken);

		public async Task<string> ReadAsBase64Async(string path, CancellationToken cancellationToken = default)
			=> Convert.ToBase64String(await ReadAllBytesAsync(path, cancellationToken));

		public string ReadAsBase64(string path, CancellationToken cancellationToken = default)
			=> Convert.ToBase64String(ReadAllBytes(path, cancellationToken));

		public async Task<string> ReadAllTextAsync(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			var bytes = await ReadAllBytesAsync(path, cancellationToken);
			return encoding.GetString(bytes);
		}

		public string ReadAllText(string path, Encoding encoding = null, CancellationToken cancellationToken = default)
		{
			if (encoding == null)
				encoding = Encoding.UTF8;

			var bytes = ReadAllBytes(path, cancellationToken);
			return encoding.GetString(bytes);
		}

		#endregion


		#region UploadFile

		public async Task UploadFileAsync(string path, byte[] buffer, CancellationToken cancellationToken = default)
			=> await ExecuteAsync(WebRequestMethods.Ftp.UploadFile, path, cancellationToken, buffer);

		public void UploadFile(string path, byte[] buffer, CancellationToken cancellationToken = default)
			=> Execute(WebRequestMethods.Ftp.UploadFile, path, cancellationToken, buffer);

		#endregion


		#region DeleteFile

		public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
			=> await ExecuteAsync(WebRequestMethods.Ftp.DeleteFile, path, cancellationToken);

		public void DeleteFile(string path, CancellationToken cancellationToken = default)
			=> Execute(WebRequestMethods.Ftp.DeleteFile, path, cancellationToken);

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

		protected void Execute(string method, string path, CancellationToken cancellationToken, byte[] buffer = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			FtpWebRequest req = GetRequest(path, method);

			void run()
			{
				if (method == WebRequestMethods.Ftp.UploadFile)
				{
					if (buffer == null)
						throw new Exception("buffer cannot be null");

					Stream requestStream = req.GetRequestStream();
					requestStream.Write(buffer, 0, buffer.Length);
					requestStream.Flush();
					requestStream.Close();
				}
				else
				{
					FtpWebResponse res = (FtpWebResponse)(req.GetResponse());
					res.Close();
					res.Dispose();
				}
			}

			bool catchHandle = method == WebRequestMethods.Ftp.MakeDirectory || method == FileExistsMethod;
			void catchHandler()
			{
				try
				{
					run();
				}
				catch (WebException ex)
				{
					FtpWebResponse res = (FtpWebResponse)ex.Response;

					if (catchHandle && res.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
						return;
					//else if (method == WebRequestMethods.Ftp.GetDateTimestamp)
					//{
					//    // do nothing
					//}
					else
						throw;
				}
			}

			if (catchHandle)
				catchHandler();
			else
				run();
		}

		protected async Task ExecuteAsync(string method, string path, CancellationToken cancellationToken, byte[] buffer = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			FtpWebRequest req = GetRequest(path, method);

			async Task run()
			{
				if (method == WebRequestMethods.Ftp.UploadFile)
				{
					if (buffer == null)
						throw new Exception("buffer cannot be null");

					Stream requestStream = req.GetRequestStream();
					requestStream.Write(buffer, 0, buffer.Length);
					requestStream.Flush();
					requestStream.Close();
				}
				else
				{
					FtpWebResponse res = (FtpWebResponse)(await req.GetResponseAsync());
					res.Close();
					res.Dispose();
				}
			}

			bool catchHandle = method == WebRequestMethods.Ftp.MakeDirectory || method == FileExistsMethod;
			async Task catchHandler()
			{
				try
				{
					await run();
				}
				catch (WebException ex)
				{
					FtpWebResponse res = (FtpWebResponse)ex.Response;

					if (catchHandle && res.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
						return;
					//else if (method == WebRequestMethods.Ftp.GetDateTimestamp)
					//{
					//    // do nothing
					//}
					else
						throw;
				}
			}

			if (catchHandle)
				await catchHandler();
			else
				await run();
		}

		protected async Task CreateFTPDirectory(string directory, CancellationToken cancellationToken)
		{
			directory = AddForwardSlashToStartAndTrim(directory);

			string[] steps = directory.Split('/', StringSplitOptions.RemoveEmptyEntries);

			// Each directory is added to the list in turn
			// directory = /a/b/c/ --> paths = /a, /a/b, /a/b/c
			List<string> paths = new List<string>();

			for (int i = 1; i <= steps.Length; i++)
				paths.Add(ForwardSlash + String.Join(ForwardSlash, steps, 0, i));

			int createIndex;
			for (createIndex = paths.Count; createIndex > 0; createIndex--)
				if (await DirectoryExistsAsync(paths[createIndex - 1], cancellationToken))
					break;

			for (; createIndex < paths.Count; createIndex++)
			{
				FtpWebRequest req = GetRequest(paths[createIndex], WebRequestMethods.Ftp.MakeDirectory);
				FtpWebResponse response = (FtpWebResponse)await req.GetResponseAsync();
				response.Close();
				response.Dispose();
			}
		}

		protected void CreateFTPDirectory(string directory)
		{
			directory = AddForwardSlashToStartAndTrim(directory);

			string[] steps = directory.Split('/', StringSplitOptions.RemoveEmptyEntries);

			// Each directory is added to the list in turn
			// directory = /a/b/c/ --> paths = /a, /a/b, /a/b/c
			List<string> paths = new List<string>();

			for (int i = 1; i <= steps.Length; i++)
				paths.Add(ForwardSlash + String.Join(ForwardSlash, steps, 0, i));

			int createIndex;
			for (createIndex = paths.Count; createIndex > 0; createIndex--)
				if (DirectoryExists(paths[createIndex - 1]))
					break;

			for (; createIndex < paths.Count; createIndex++)
			{
				FtpWebRequest req = GetRequest(paths[createIndex], WebRequestMethods.Ftp.MakeDirectory);
				FtpWebResponse response = (FtpWebResponse)req.GetResponse();
				response.Close();
				response.Dispose();
			}
		}

		protected static string AddForwardSlashToStartAndTrim(string directory)
		{
			directory = directory.Trim();

			if (directory.StartsWith(ForwardSlash))
				return directory;

			return ForwardSlash + directory;
		}

		protected FtpWebRequest GetRequest(string path, string method, bool useBinary = true)
		{
			Uri uri = GetUri(path);
			var req = (FtpWebRequest)FtpWebRequest.Create(uri);
			req.Method = method;
			req.UseBinary = useBinary;
			req.Credentials = Credential;
			return req;
		}

		protected async Task<byte[]> DownloadDataTaskAsync(Uri address, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// TODO: not working as expected!
			// https://github.com/dotnet/runtime/issues/31479
			// https://stackoverflow.com/questions/48187976/webclient-cancelasync-file-still-downloading
			using (cancellationToken.Register(WebClient.CancelAsync))
			{
				return await WebClient.DownloadDataTaskAsync(address);
			}
		}

		protected byte[] DownloadData(Uri address, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// TODO: not working as expected!
			// https://github.com/dotnet/runtime/issues/31479
			// https://stackoverflow.com/questions/48187976/webclient-cancelasync-file-still-downloading
			using (cancellationToken.Register(WebClient.CancelAsync))
			{
				return WebClient.DownloadData(address);
			}
		}

		protected Uri GetUri(string path)
		{
			string url = string.Format("{0}/{1}", BaseUrl.TrimEnd('/'), path.TrimStart('/'));
			Uri uri = new(url);

			if (uri.Scheme != Uri.UriSchemeFtp)
				throw new InvalidOperationException($"Url is not equal to UriSchemeFtp. Url: {url}");

			return uri;
		}

		#endregion


		#region IDisposable
		private bool _disposed = false;
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				// Dispose Managed Resources
				_webClient?.Dispose();
			}

			// Dispose Native Resources

			_disposed = true;
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~FtpSender()
		{
			Dispose(false);
		}
		#endregion
	}
}
