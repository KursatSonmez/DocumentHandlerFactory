using DocumentHandlerFactory.Models;
using DocumentHandlerFactory.Senders;
using DocumentHandlerFactory.Senders.Interfaces;
using DocumentHandlerFactory.Settings;
using System;

namespace DocumentHandlerFactory.Handlers.Base
{
    public abstract class BaseDocumentHandler : IDisposable
    {
        protected readonly DocumentHandlerSettings Settings;

        public BaseDocumentHandler(DocumentHandlerSettings settings)
        {
            Settings = settings;
            CreateSender();
        }

        protected HandlerType HandlerType => Settings.HandlerType;

        private FileSender _fileSender;
        private FtpSender _ftpSender;
        protected ISender Sender
        {
            get => HandlerType switch
            {
                HandlerType.File => _fileSender,
                HandlerType.Ftp => _ftpSender,
                _ => throw new ArgumentException(message: $"invalid enum name! value: {HandlerType}", paramName: nameof(HandlerType))
            };
            private set
            {
                switch (HandlerType)
                {
                    case HandlerType.File:
                        _fileSender = (FileSender)value;
                        break;

                    case HandlerType.Ftp:
                        _ftpSender = (FtpSender)value;
                        break;

                    default:
                        throw new ArgumentException(message: $"invalid enum name! value: {HandlerType}", paramName: "HandlerType.value");
                }
            }
        }

        private void CreateSender()
            => Sender = HandlerType switch
            {
                HandlerType.File => new FileSender(Settings),
                HandlerType.Ftp => new FtpSender(Settings),
                _ => throw new ArgumentException(message: $"invalid enum name! value: {HandlerType}", paramName: nameof(HandlerType))
            };



        #region IDisposable
        /// <summary>
        /// Dispose managed state (managed objects) (e.g x.Dispose())
        /// </summary>
        protected abstract void DisposeManagedResources();

        /// <summary>
        /// Free unmanaged resources (unmanaged objects).
        /// Set large fields to null.
        /// </summary>
        protected abstract void DisposeNativeResources();

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Dispose Managed Resources
                Sender.Dispose();

                DisposeManagedResources();
            }

            // Dispose Native Resources

            DisposeNativeResources();

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseDocumentHandler()
        {
            Dispose(false);
        }
        #endregion
    }
}
