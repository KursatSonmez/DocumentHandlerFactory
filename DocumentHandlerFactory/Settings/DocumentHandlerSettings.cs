using DocumentHandlerFactory.Extensions;
using DocumentHandlerFactory.Models;
using System;

namespace DocumentHandlerFactory.Settings
{
    public class DocumentHandlerSettings
    {
        public DocumentHandlerSettings()
        {
        }

        /// <summary>
        /// Represents the default rendering format.
        /// 
        /// Unless overridden, all transactions are processed by the type specified here.
        /// 
        /// Default: <see cref="HandlerType.File"/>
        /// </summary>
        public HandlerType HandlerType { get; set; }

        public FtpSettings FtpSettings { get; set; }

        public FileSettings FileSettings { get; set; }

        public static DocumentHandlerSettings DefaultValue()
        {
            return new DocumentHandlerSettings()
            {
                HandlerType = HandlerType.File,
                FileSettings = new FileSettings(),
                FtpSettings = null,
            };
        }
        private void SetToDefaultValues()
        {
            var val = DefaultValue();

            this.HandlerType = val.HandlerType;
            this.FtpSettings = val.FtpSettings;
            this.FileSettings = val.FileSettings;
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

        private void ValidateFtp()
        {
            var settings = this;

            if (settings.FtpSettings == null)
                throw new ArgumentNullException(message: "FtpSettings is null!", paramName: nameof(settings.FtpSettings));
        }

        private void ValidateFile()
        {
            var settings = this;

            if (settings.FileSettings == null)
                throw new ArgumentNullException(message: "FileSettings is null!", paramName: nameof(settings.FileSettings));

            if (FileSettings.BaseDirectory == null)
                throw new ArgumentNullException(paramName: nameof(FileSettings.BaseDirectory));

            if (!BetterPath.IsValiFilePath(FileSettings.BaseDirectory))
                throw new InvalidOperationException($"FileSettings.BaseDirectory could not be verified!");
        }
    }
}
