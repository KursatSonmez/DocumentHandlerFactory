using DocumentHandlerFactory.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DocumentHandlerFactory.UnitTest.HandlerTests
{
	public class DocumentHandlerSyncTests : BaseDocumentHandlerTests
	{
		public override string TestFolderName => "_TestFilesSync";


		[Fact]
		public void FileExists()
		{
			var (handler, _) = GetHandler();

			var res = handler.FileExists("file1.txt");

			Assert.True(res);
		}


		[Fact]
		public void DirectoryExists()
		{
			var (handler, _) = GetHandler();

			var res = handler.DirectoryExists("");

			Assert.True(res);
		}

		[Fact]
		public void CreateDirectory()
		{
			var (handler, settings) = GetHandler();

			string dirName = Guid.NewGuid().ToString();

			// create directory
			var ex = Record.Exception(() => handler.CreateDirectory(dirName));

			Assert.Null(ex);

			// check directory existing
			var res = handler.DirectoryExists(dirName);

			Assert.True(res);

			// delete directory
			// TODO: delete with handler
			string dirFullPath = BetterPath.Combine(settings.FileSettings.BaseDirectory, dirName);

			Directory.Delete(dirFullPath);
		}

		[Fact]
		public void ReadFileTests()
		{
			var (handler, _) = GetHandler();

			// bytes
			var bytes = handler.ReadAllBytes("file1.txt");

			Assert.True(bytes.Length > 0);

			// base64
			var base64 = handler.ReadAsBase64("file1.txt");

			Assert.False(string.IsNullOrWhiteSpace(base64));

			// text
			var text = handler.ReadAllText("file1.txt");

			Assert.Equal("file1", text.Trim());
		}

		[Fact]
		public void UploadFile_and_DeleteFile()
		{
			var (handler, _) = GetHandler();

			string fileName = Guid.NewGuid().ToString() + ".txt";

			string text = "file2" + fileName;

			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);

			// upload file
			var ex = Record.Exception(() => handler.UploadFile(fileName, bytes));

			Assert.Null(ex);

			//string fullPath = BetterPath.Combine(settings.FileSettings.BaseDirectory, fileName);

			// check file existing
			Assert.True(
					handler.FileExists(fileName)
				);

			// delete file
			ex = Record.Exception(() => handler.DeleteFile(fileName));

			Assert.Null(ex);

			// check file existing
			Assert.False(
					handler.FileExists(fileName)
				);
		}

		[Fact]
		public void CopyFile_and_DeleteFile()
		{
			var (handler, _) = GetHandler();

			string fileName = Guid.NewGuid().ToString() + ".txt";

			// copy file
			var ex = Record.Exception(() => handler.CopyFile("file1.txt", fileName));

			Assert.Null(ex);

			// delete file
			ex = Record.Exception(() => handler.DeleteFile(fileName));

			Assert.Null(ex);
		}
	}
}
