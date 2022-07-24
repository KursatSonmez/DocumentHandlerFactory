using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Handlers;
using DocumentHandlerFactory.Handlers.Interfaces;
using DocumentHandlerFactory.Settings;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DocumentHandlerFactory.UnitTest.HandlerTests
{
    public class DocumentHandlerTests
    {
        public const string TestFolderName = "_TestFiles";


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
            // TODO: delete with handler
            string dirFullPath = BetterPath.Combine(settings.FileSettings.BaseDirectory, dirName);

            Directory.Delete(dirFullPath);
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



        private string _baseDirectory;
        private readonly object _baseDirectory_locker = new();
        private string BaseDirectory
        {
            get
            {
                if (_baseDirectory != null)
                    return _baseDirectory;

                lock (_baseDirectory_locker)
                {
                    if (_baseDirectory == null)
                    {
                        var assemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
                        var baseDirectory =
                            Path.Combine(
                                Path.GetDirectoryName(assemblyLocation),
                                TestFolderName
                            );
                        _baseDirectory = baseDirectory;
                    }
                }

                return _baseDirectory;
            }
        }

        private (IDocumentHandler, DocumentHandlerSettings) GetHandler()
        {
            var settings = DocumentHandlerSettings.DefaultValue();

            settings.FileSettings = new FileSettings(BaseDirectory);

            var handler = new DocumentHandler(settings);

            return (handler, settings);
        }
    }
}
