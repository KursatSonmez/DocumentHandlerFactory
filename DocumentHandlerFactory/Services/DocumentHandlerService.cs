using DocumentHandlerFactory.Handlers;
using DocumentHandlerFactory.Services.Interfaces;
using DocumentHandlerFactory.Settings;

namespace DocumentHandlerFactory.Services
{
    public class DocumentHandlerService : DocumentHandler, IDocumentHandlerService
    {
        public DocumentHandlerService(DocumentHandlerSettings settings) : base(settings)
        {
        }
    }
}
