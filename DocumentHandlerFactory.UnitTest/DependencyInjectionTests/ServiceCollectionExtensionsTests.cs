using DocumentHandlerFactory.DependencyInjection;
using DocumentHandlerFactory.Services.Interfaces;
using DocumentHandlerFactory.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace DocumentHandlerFactory.UnitTest.DependencyInjectionTests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDocumentHandlerFactory_Should_be_Successful()
        {
            // file
            {
                var services = new ServiceCollection().
                    AddDocumentHandlerFactory();

                var provider = services.BuildServiceProvider();

                // DocumentHandlerSettings
                var ex = Record.Exception(() => provider.GetRequiredService<DocumentHandlerSettings>());

                Assert.Null(ex);

                var settings = provider.GetRequiredService<DocumentHandlerSettings>();
                Assert.NotNull(settings.FileSettings);

                // IDocumentHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<IDocumentHandlerService>());

                Assert.Null(ex);
            }

            // file - custom path
            {
                var services = new ServiceCollection()
                    .AddDocumentHandlerFactory(x => x.FileSettings = new FileSettings(baseDirectory: "base-directory"));

                var provider = services.BuildServiceProvider();

                // DocumentHandlerSettings
                var ex = Record.Exception(() => provider.GetRequiredService<DocumentHandlerSettings>());

                Assert.Null(ex);

                var settings = provider.GetRequiredService<DocumentHandlerSettings>();
                Assert.NotNull(settings.FileSettings);
                Assert.Equal("base-directory", settings.FileSettings.BaseDirectory);

                // IDocumentHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<IDocumentHandlerService>());

                Assert.Null(ex);
            }

            // ftp
            {
                var services = new ServiceCollection().
                    AddDocumentHandlerFactory(x =>
                    {
                        x.HandlerType = Models.HandlerType.Ftp;
                        x.FtpSettings = new FtpSettings("ftp-url", "ftp-username", "ftp-password");
                        x.FileSettings = null;
                    });

                var provider = services.BuildServiceProvider();

                // DocumentHandlerSettings
                var ex = Record.Exception(() => provider.GetRequiredService<DocumentHandlerSettings>());

                Assert.Null(ex);

                var settings = provider.GetRequiredService<DocumentHandlerSettings>();
                Assert.NotNull(settings.FtpSettings);
                Assert.Equal("ftp-url", settings.FtpSettings.Url);
                Assert.Equal("ftp-username", settings.FtpSettings.Username);
                Assert.Equal("ftp-password", settings.FtpSettings.Password);


                // IDocumentHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<IDocumentHandlerService>());

                Assert.Null(ex);
            }
        }

        [Fact]
        public void AddDocumentHandlerFactory_Should_be_Fail()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var services = new ServiceCollection().
                    AddDocumentHandlerFactory(x => x.FileSettings = null);
            });

            Assert.Equal(nameof(DocumentHandlerSettings.FileSettings), ex.ParamName);


            ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var services = new ServiceCollection().
                    AddDocumentHandlerFactory(x =>
                    {
                        x.HandlerType = Models.HandlerType.Ftp;
                        x.FtpSettings = null;
                    });
            });

            Assert.Equal(nameof(DocumentHandlerSettings.FtpSettings), ex.ParamName);


            var exTempFileHandlerService = Record.Exception(() =>
            {
                var services = new ServiceCollection()
                    .AddDocumentHandlerFactory();

                var provider = services.BuildServiceProvider();

                _ = provider.GetRequiredService<ITempFileHandlerService>();
            });

            Assert.NotNull(exTempFileHandlerService);
            Assert.Equal("No service for type 'DocumentHandlerFactory.Services.Interfaces.ITempFileHandlerService' has been registered.", exTempFileHandlerService.Message);
        }

        [Fact]
        public void AddTempFileHandler_Should_be_Successful()
        {
            // file
            {
                var services = new ServiceCollection()
                    .AddTempFileHandler();

                var provider = services.BuildServiceProvider();

                // TempFileSettings
                var ex = Record.Exception(() => provider.GetRequiredService<TempFileSettings>());

                Assert.Null(ex);

                var settings = provider.GetRequiredService<TempFileSettings>();
                Assert.NotNull(settings.FileSettings);
                Assert.EndsWith(TempFileSettings.DefaultTempFolderName, settings.FileSettings.BaseDirectory);
                Assert.Equal(TempFileSettings.DefaultTempFolderName, settings.TempFolderName);

                // ITempFileHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<ITempFileHandlerService>());

                Assert.Null(ex);
            }

            // file - custom path
            {
                var services = new ServiceCollection()
                    .AddTempFileHandler(x => x.FileSettings = new FileSettings(baseDirectory: $"base-directory\\{x.TempFolderName}"));

                var provider = services.BuildServiceProvider();

                // TempFileSettings
                var ex = Record.Exception(() => provider.GetRequiredService<TempFileSettings>());

                Assert.Null(ex);

                var settings = provider.GetRequiredService<TempFileSettings>();
                Assert.NotNull(settings.FileSettings);
                Assert.Equal($"base-directory\\{TempFileSettings.DefaultTempFolderName}", settings.FileSettings.BaseDirectory);
                Assert.Equal(TempFileSettings.DefaultTempFolderName, settings.TempFolderName);

                // ITempFileHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<ITempFileHandlerService>());

                Assert.Null(ex);
            }

            // ftp
            {
                var services = new ServiceCollection()
                    .AddTempFileHandler(x =>
                    {
                        x.HandlerType = Models.HandlerType.Ftp;
                        x.FtpSettings = new FtpSettings("ftp-url", "ftp-username", "ftp-password");
                    });

                var provider = services.BuildServiceProvider();

                // TempFileSettings
                var ex = Record.Exception(() => provider.GetRequiredService<TempFileSettings>());

                Assert.Null(ex);

                var settings = provider.GetRequiredService<TempFileSettings>();
                Assert.NotNull(settings.FtpSettings);
                Assert.Equal("ftp-url", settings.FtpSettings.Url);
                Assert.Equal("ftp-username", settings.FtpSettings.Username);
                Assert.Equal("ftp-password", settings.FtpSettings.Password);

                // ITempFileHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<ITempFileHandlerService>());

                Assert.Null(ex);
            }
        }

        [Fact]
        public void AddTempFileHandler_Should_be_Fail()
        {
            // FileSettings is null
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var services = new ServiceCollection()
                    .AddTempFileHandler(x => x.FileSettings = null);
            });

            Assert.Equal(nameof(TempFileSettings.FileSettings), ex.ParamName);

            // FileSettings invalid BaseDirectory
            var exInvalid = Assert.Throws<InvalidOperationException>(() =>
            {
                var services = new ServiceCollection().
                    AddTempFileHandler(x => x.FileSettings.SetBaseDirectory("base-directory"));
            });

            Assert.Equal("FileSettings.BaseDirectory must end with TempFolderName!", exInvalid.Message);

            // FtpSettings is null
            ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var services = new ServiceCollection().
                    AddTempFileHandler(x =>
                    {
                        x.HandlerType = Models.HandlerType.Ftp;
                        x.FtpSettings = null;
                    });
            });

            Assert.Equal(nameof(TempFileSettings.FtpSettings), ex.ParamName);
        }
    }
}
