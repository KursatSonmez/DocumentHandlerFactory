using System;

namespace DocumentHandlerFactory.Settings
{
    /// <summary>
    /// Contains the necessary settings for the operations to file system
    /// </summary>
    public class FileSettings
    {
        public FileSettings()
        {
            var assemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;

            var baseDirectory =
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(assemblyLocation),
                    "Handled Documents"
                );

            SetBaseDirectory(baseDirectory);
        }

        public FileSettings(string baseDirectory)
        {
            SetBaseDirectory(baseDirectory);
        }

        /// <summary>
        /// Specifies the base folder which files will be processed (upload, delete etc.).
        /// All files are saved under this directory.
        /// <br />
        /// Default: The default value is "{the name of the folder where the assembly is located}/Handled Documents"
        /// </summary>
        public string BaseDirectory { get; private set; }

        private void SetBaseDirectory(string baseDirectory)
            => BaseDirectory = baseDirectory ?? throw new ArgumentNullException(paramName: nameof(baseDirectory));
    }
}
