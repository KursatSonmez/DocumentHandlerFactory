using DocumentHandlerFactory.Models;
using DocumentHandlerFactory.Settings;
using System;
using System.IO;
using Xunit;

namespace DocumentHandlerFactory.UnitTest.Settings
{
    public class TempFileSettingsTests
    {
        [Fact]
        public void Default_Initialization_Should_be_Successful()
        {
            var settings = TempFileSettings.DefaultValue();

            Assert.Equal(HandlerType.File, settings.HandlerType);

            Assert.Null(settings.FtpSettings);
            Assert.NotNull(settings.FileSettings);

            // FileSettings
            {
                var baseDir = Path.Combine(Path.GetTempPath(), TempFileSettings.DefaultTempFolderName);

                Assert.Equal(baseDir, settings.FileSettings.BaseDirectory);
            }
        }

        [Fact]
        public void HandlerType_Validation_Should_be_Fail()
        {
            // Ftp validation
            {
                var settings = new TempFileSettings
                {
                    HandlerType = HandlerType.Ftp,
                    FtpSettings = null,
                    FileSettings = new FileSettings(),
                    TempFolderName = TempFileSettings.DefaultTempFolderName,
                };

                var ex = Assert.Throws<ArgumentNullException>(() => settings.Validate());

                Assert.Equal(nameof(TempFileSettings.FtpSettings), ex.ParamName);
            }

            // File validation
            {
                var settings = new TempFileSettings
                {
                    HandlerType = HandlerType.File,
                    FtpSettings = null,
                    FileSettings = null,
                };

                var ex = Assert.Throws<ArgumentNullException>(() => settings.Validate());

                Assert.Equal(nameof(TempFileSettings.FileSettings), ex.ParamName);
            }
        }

        [Theory]
        [InlineData("url", null, "username", "password")]
        [InlineData("url", "", "username", "password")]
        [InlineData("url", " ", "username", "password")]

        [InlineData("username", "url", null, "password")]
        [InlineData("username", "url", "", "password")]
        [InlineData("username", "url", " ", "password")]

        [InlineData("password", "url", "username", null)]
        [InlineData("password", "url", "username", "")]
        [InlineData("password", "url", "username", " ")]
        public void FtpSettings_Validation_Should_be_Fail(string checkParamName, string url, string username, string password)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var settings = new TempFileSettings
                {
                    HandlerType = HandlerType.Ftp,
                    FtpSettings = new FtpSettings(url: url, username: username, password: password),
                    FileSettings = null,
                    TempFolderName = null,
                };
                settings.Validate();
            });

            Assert.Equal(checkParamName, ex.ParamName);
        }

        [Theory]
        [InlineData("https://", "username", "passW0r?+^%'?+^=?'D/d")]
        [InlineData("url", "u", "213421")]
        [InlineData("asd", "asd", "asd")]
        public void FtpSettings_Validation_Should_be_Successful(string url, string username, string password)
        {
            var settings = new TempFileSettings
            {
                HandlerType = HandlerType.Ftp,
                FtpSettings = new FtpSettings(url: url, username: username, password: password),
                FileSettings = null,
            };

            var ex = Record.Exception(() => settings.Validate());

            Assert.Null(ex);
        }

        [Theory]
        [InlineData("BaseDirectory", null, "tempfolder", "ArgumentNullException for baseDirectory")]
        [InlineData("", "C:/asdasdasd/d|jfkdjf", "tempfolder", "FileSettings.BaseDirectory could not be verified!")]
        [InlineData("TempFolderName", "C:/asdasdasd/djfkdjf", "", "ArgumentNullException for tempFolderName")]
        [InlineData("", "C:/asdasdasd/djfkdjf", "tempfolder", "FileSettings.BaseDirectory must end with TempFolderName!")]
        public void FileSettings_Validation_Should_Be_Fail(string checkParamName, string baseDirectory, string tempFolderName, string errorMessage)
        {
            try
            {
                var settings = new TempFileSettings
                {
                    HandlerType = HandlerType.File,
                    FileSettings = new FileSettings(baseDirectory: baseDirectory),
                    FtpSettings = null,
                    TempFolderName = tempFolderName,
                };
                settings.Validate();

                throw new Exception("no error was thrown!");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Equal(errorMessage, ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                Assert.Equal(checkParamName, ex.ParamName);
            }
        }

        [Theory]
        [InlineData("tempfolder", "tempfolder")]
        [InlineData(" tempfolder", " tempfolder")]
        [InlineData("asolfkjasl/tempfolder", "tempfolder")]
        [InlineData("C:/asdasdasd/asdqweqwxööcv/asdasfasftempfolder", "asdasfasftempfolder")]
        [InlineData(@"C:\asldksaqwe\asldfkasf", "asldfkasf")]
        [InlineData("C:\\AA\\BB\\CC\\CCtempfolder", "CCtempfolder")]
        public void FileSettings_Validation_Should_Be_Successful(string baseDirectory, string tempFolderName)
        {
            var settings = new TempFileSettings
            {
                HandlerType = HandlerType.File,
                FileSettings = new FileSettings(baseDirectory: baseDirectory),
                FtpSettings = null,
                TempFolderName = tempFolderName,
            };

            var ex = Record.Exception(() => settings.Validate());

            Assert.Null(ex);
        }

    }
}
