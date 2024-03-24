using DocumentHandlerFactory.Handlers.Interfaces;
using DocumentHandlerFactory.Handlers;
using DocumentHandlerFactory.Settings;
using System.IO;

namespace DocumentHandlerFactory.UnitTest.HandlerTests
{
	public abstract class BaseDocumentHandlerTests
	{
		public abstract string TestFolderName { get; }


		private string _baseDirectory;
		private readonly object _baseDirectory_locker = new();
		protected string BaseDirectory
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

		protected virtual (IDocumentHandler, DocumentHandlerSettings) GetHandler()
		{
			var settings = DocumentHandlerSettings.DefaultValue();

			settings.FileSettings = new FileSettings(BaseDirectory);

			var handler = new DocumentHandler(settings);

			return (handler, settings);
		}
	}
}
