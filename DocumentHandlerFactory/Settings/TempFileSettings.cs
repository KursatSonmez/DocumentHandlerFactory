using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Models;
using System;
using System.IO;

namespace DocumentHandlerFactory.Settings
{
    public class TempFileSettings
    {
        public const string DefaultTempFolderName = "DocumentHandlerFactory.TempFileHandler.66319f62-1cbf-432c-9684-43761f7a483b";

        /// <summary>
        /// Represents the default rendering format for temp file operations.
        /// 
        /// Unless overridden, all transactions are processed by the type specified here.
        /// 
        /// Default: <see cref="HandlerType.File"/>
        /// </summary>
        public HandlerType HandlerType { get; set; }

        public FtpSettings FtpSettings { get; set; }

        public FileSettings FileSettings { get; set; }

        /// <summary>
        /// Is the temporary folder name of the application in which the library is used.
        /// 
        /// Default: "DocumentHandlerFactory.TempFileHandler.66319f62-1cbf-432c-9684-43761f7a483b"
        /// 
        /// <br />
        /// 
        /// <b><i>This parameter is used only when <see cref="HandlerType"/> is <see cref="HandlerType.File"/>. </i></b>
        /// </summary>
        public string TempFolderName { get; set; }


        public static TempFileSettings DefaultValue()
        {
            string baseDir = Path.Combine(Path.GetTempPath(), DefaultTempFolderName);

            return new TempFileSettings
            {
                HandlerType = HandlerType.File,
                FileSettings = new FileSettings(baseDirectory: baseDir),
                FtpSettings = null,
                TempFolderName = DefaultTempFolderName,
            };
        }

        public static explicit operator DocumentHandlerSettings(TempFileSettings obj)
        {
            return new DocumentHandlerSettings
            {
                FileSettings = obj.FileSettings,
                FtpSettings = obj.FtpSettings,
                HandlerType = obj.HandlerType,
            };
        }

        public void Validate()
        {
            switch (HandlerType)
            {
                case HandlerType.File:
                    ValidateFile();
                    break;
                case HandlerType.Ftp:
                    ValidateFtp();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void ValidateFtp()
        {
            if (FtpSettings == null)
                throw new ArgumentNullException(paramName: nameof(FtpSettings));
        }

        public void ValidateFile()
        {
            if (FileSettings == null)
                throw new ArgumentNullException(paramName: nameof(FileSettings));

            if (string.IsNullOrWhiteSpace(TempFolderName))
                throw new ArgumentNullException(paramName: nameof(TempFolderName));

            if (!BetterPath.IsValiFilePath(FileSettings.BaseDirectory))
                throw new InvalidOperationException("FileSettings.BaseDirectory could not be verified!");

            if (!FileSettings.BaseDirectory.EndsWith(TempFolderName))
                throw new InvalidOperationException("FileSettings.BaseDirectory must end with TempFolderName!");
        }
    }
}
