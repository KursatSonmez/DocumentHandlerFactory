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

                // ITempFileHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<ITempFileHandlerService>());

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

                // ITempFileHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<ITempFileHandlerService>());

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

                // ITempFileHandlerService
                ex = Record.Exception(() => provider.GetRequiredService<ITempFileHandlerService>());

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
        }
    }
}
