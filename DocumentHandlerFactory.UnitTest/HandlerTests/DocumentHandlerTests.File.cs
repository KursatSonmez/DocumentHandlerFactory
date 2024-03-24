using DocumentHandlerFactory.Extensions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DocumentHandlerFactory.UnitTest.HandlerTests
{
	public class DocumentHandlerTests : BaseDocumentHandlerTests
	{
		public override string TestFolderName => "_TestFiles";


		[Fact]
		public async Task FileExistsAsync()
		{
			var (handler, _) = GetHandler();

			var res = await handler.FileExistsAsync("file1.txt");

			Assert.True(res);
		}


		[Fact]
		public async Task DirectoryExistsAsync()
		{
			var (handler, _) = GetHandler();

			var res = await handler.DirectoryExistsAsync("");

			Assert.True(res);
		}

		[Fact]
		public async Task CreateDirectoryAsync()
		{
			var (handler, settings) = GetHandler();

			string dirName = Guid.NewGuid().ToString();

			// create directory
			var ex = await Record.ExceptionAsync(async () => await handler.CreateDirectoryAsync(dirName));

			Assert.Null(ex);

			// check directory existing
			var res = await handler.DirectoryExistsAsync(dirName);

			Assert.True(res);

			// delete directory
			string dirFullPath = BetterPath.Combine(settings.FileSettings.BaseDirectory, dirName);

			ex = await Record.ExceptionAsync(async () => await handler.DeleteDirectoryAsync(dirFullPath));

			Assert.Null(ex);
		}

		[Fact]
		public async Task ReadFileTests()
		{
			var (handler, _) = GetHandler();

			// bytes
			var bytes = await handler.ReadAllBytesAsync("file1.txt");

			Assert.True(bytes.Length > 0);

			// base64
			var base64 = await handler.ReadAsBase64Async("file1.txt");

			Assert.False(string.IsNullOrWhiteSpace(base64));

			// text
			var text = await handler.ReadAllTextAsync("file1.txt");

			Assert.Equal("file1", text.Trim());
		}

		[Fact]
		public async Task UploadFileAsync_and_DeleteFileAsync()
		{
			var (handler, settings) = GetHandler();

			string fileName = Guid.NewGuid().ToString() + ".txt";

			string text = "file2" + fileName;

			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

			// upload file
			var ex = await Record.ExceptionAsync(async () => await handler.UploadFileAsync(fileName, bytes));

			Assert.Null(ex);

			//string fullPath = BetterPath.Combine(settings.FileSettings.BaseDirectory, fileName);

			// check file existing
			Assert.True(
					await handler.FileExistsAsync(fileName)
				);

			// delete file
			ex = await Record.ExceptionAsync(async () => await handler.DeleteFileAsync(fileName));

			Assert.Null(ex);

			// check file existing
			Assert.False(
					await handler.FileExistsAsync(fileName)
				);
		}

		[Fact]
		public async Task CopyFileAsync_and_DeleteFileAsync()
		{
			var (handler, settings) = GetHandler();

			string fileName = Guid.NewGuid().ToString() + ".txt";

			// copy file
			var ex = await Record.ExceptionAsync(async () => await handler.CopyFileAsync("file1.txt", fileName));

			Assert.Null(ex);

			// delete file
			ex = await Record.ExceptionAsync(async () => await handler.DeleteFileAsync(fileName));

			Assert.Null(ex);
		}

	}
}
