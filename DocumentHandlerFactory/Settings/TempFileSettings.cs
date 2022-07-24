using DocumentHandlerFactory.Extensions;
using System;

namespace DocumentHandlerFactory.Settings
{
    public class TempFileSettings
    {
        public TempFileSettings()
        {
            SetTempFileSettings(
                    tempFolderBasePath: System.IO.Path.GetTempPath(),
                    tempFolderName: "DocumentHandlerFactory.TempFileHandler.66319f62-1cbf-432c-9684-43761f7a483b"
                );
        }

        public TempFileSettings(string tempFolderName)
        {
            SetTempFileSettings(
                    tempFolderBasePath: System.IO.Path.GetTempPath(),
                    tempFolderName: tempFolderName
                );
        }

        public TempFileSettings(string tempFolderBasePath, string tempFolderName)
        {
            SetTempFileSettings(tempFolderBasePath, tempFolderName);
        }

        /// <summary>
        /// Default: <see cref="System.IO.Path.GetTempPath"/>
        /// </summary>
        public string TempFolderBasePath { get; private set; }

        /// <summary>
        /// Is the temporary folder name of the application in which the library is used.
        /// 
        /// Default: "DocumentHandlerFactory.TempFileHandler.66319f62-1cbf-432c-9684-43761f7a483b"
        /// </summary>
        public string TempFolderName { get; private set; }

        /// <summary>
        /// Represents the "TempFolderBasePath + TempFolderName" value. Returns the entire temp folder path.
        /// </summary>
        public string TempFolderFullPath => BetterPath.Combine(TempFolderBasePath, TempFolderName);


        private void SetTempFileSettings(string tempFolderBasePath, string tempFolderName)
        {
            if (string.IsNullOrWhiteSpace(tempFolderBasePath))
                throw new ArgumentNullException(paramName: nameof(tempFolderBasePath));

            if (tempFolderName == null)
                throw new ArgumentNullException(paramName: nameof(tempFolderName));

            TempFolderBasePath = tempFolderBasePath;
            TempFolderName = tempFolderName;
        }
    }
}
