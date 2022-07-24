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

        public TempFileSettings TempFileSettings { get; set; }

        public static DocumentHandlerSettings DefaultValue()
        {
            return new DocumentHandlerSettings()
            {
                HandlerType = HandlerType.File,
                FileSettings = new FileSettings(),
                FtpSettings = null,
                TempFileSettings = new TempFileSettings(),
            };
        }
        private void SetToDefaultValues()
        {
            var val = DefaultValue();

            this.HandlerType = val.HandlerType;
            this.FtpSettings = val.FtpSettings;
            this.FileSettings = val.FileSettings;
            this.TempFileSettings = val.TempFileSettings;
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

            if (TempFileSettings == null)
                throw new ArgumentNullException(paramName: nameof(TempFileSettings));
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
        }
    }
}
