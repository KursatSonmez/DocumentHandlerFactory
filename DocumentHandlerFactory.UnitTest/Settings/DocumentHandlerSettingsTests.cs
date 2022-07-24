using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Models;
using DocumentHandlerFactory.Settings;
using System;
using System.IO;
using Xunit;

namespace DocumentHandlerFactory.UnitTest.Settings
{
    public class DocumentHandlerSettingsTests
    {
        [Fact]
        public void Default_Initialization_Should_be_Successful()
        {
            var settings = DocumentHandlerSettings.DefaultValue();

            Assert.Equal(HandlerType.File, settings.HandlerType);

            Assert.Null(settings.FtpSettings);
            Assert.NotNull(settings.FileSettings);
            Assert.NotNull(settings.TempFileSettings);

            // FileSettings
            {
                var assemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;

                var baseDirectory =
                    Path.Combine(
                        Path.GetDirectoryName(assemblyLocation),
                        "Handled Documents"
                    );

                Assert.Equal(baseDirectory, settings.FileSettings.BaseDirectory);
            }

            // TempFileSettings
            {
                string tempFolderBasePath = Path.GetTempPath(),
                    tempFolderName = "DocumentHandlerFactory.TempFileHandler.66319f62-1cbf-432c-9684-43761f7a483b",
                    tempFolderFullPath = BetterPath.Combine(tempFolderBasePath, tempFolderName);

                Assert.Equal(tempFolderBasePath, settings.TempFileSettings.TempFolderBasePath);
                Assert.Equal(tempFolderName, settings.TempFileSettings.TempFolderName);
                Assert.Equal(tempFolderFullPath, settings.TempFileSettings.TempFolderFullPath);
            }
        }

        [Fact]
        public void Default_Initialization_Validation_Should_be_Fail()
        {
            // Ftp validation
            {
                var settings = new DocumentHandlerSettings
                {
                    HandlerType = HandlerType.Ftp,
                    FtpSettings = null,
                    FileSettings = new FileSettings(),
                    TempFileSettings = null,
                };

                var ex = Assert.Throws<ArgumentNullException>(() => settings.Validate());

                Assert.Equal(nameof(DocumentHandlerSettings.FtpSettings), ex.ParamName);
                Assert.StartsWith("FtpSettings is null!", ex.Message);
            }

            // File validation
            {
                var settings = new DocumentHandlerSettings
                {
                    HandlerType = HandlerType.File,
                    FtpSettings = null,
                    FileSettings = null,
                    TempFileSettings = null,
                };

                var ex = Assert.Throws<ArgumentNullException>(() => settings.Validate());

                Assert.Equal(nameof(DocumentHandlerSettings.FileSettings), ex.ParamName);
                Assert.StartsWith("FileSettings is null!", ex.Message);
            }

            // TempFileSettings validation
            {
                var settings = new DocumentHandlerSettings
                {
                    HandlerType = HandlerType.File,
                    FtpSettings = null,
                    FileSettings = new FileSettings(),
                    TempFileSettings = null,
                };

                var ex = Assert.Throws<ArgumentNullException>(() => settings.Validate());

                Assert.Equal(nameof(DocumentHandlerSettings.TempFileSettings), ex.ParamName);
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
                var settings = new DocumentHandlerSettings
                {
                    HandlerType = HandlerType.Ftp,
                    FtpSettings = new FtpSettings(url: url, username: username, password: password),
                    FileSettings = null,
                    TempFileSettings = null,
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
            var settings = new DocumentHandlerSettings
            {
                HandlerType = HandlerType.Ftp,
                FtpSettings = new FtpSettings(url: url, username: username, password: password),
                FileSettings = null,
                TempFileSettings = new TempFileSettings(),
            };

            var ex = Record.Exception(() => settings.Validate());

            Assert.Null(ex);
        }

        [Theory]
        [InlineData("baseDirectory", null)]
        public void FileSettings_Validation_Should_Be_Fail(string checkParamName, string baseDirectory)
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var settings = new DocumentHandlerSettings
                {
                    HandlerType = HandlerType.File,
                    FileSettings = new FileSettings(baseDirectory: baseDirectory),
                    FtpSettings = null,
                    TempFileSettings = new TempFileSettings(),
                };
                settings.Validate();
            });

            Assert.Equal(checkParamName, ex.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("asolfkjasl")]
        public void FileSettings_Validation_Should_Be_Successful(string baseDirectory)
        {
            var settings = new DocumentHandlerSettings
            {
                HandlerType = HandlerType.File,
                FileSettings = new FileSettings(baseDirectory: baseDirectory),
                FtpSettings = null,
                TempFileSettings = new TempFileSettings(),
            };

            var ex = Record.Exception(() => settings.Validate());

            Assert.Null(ex);
        }
    }
}
