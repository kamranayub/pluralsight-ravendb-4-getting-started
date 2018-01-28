using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Sample.Models;
using Sample.Services;

namespace Sample
{
    public interface IDocumentStoreHolder
    {
        IDocumentStore Store { get; }
    }

    public class DocumentStoreHolder : IDocumentStoreHolder
    {
        private readonly ILogger<DocumentStoreHolder> _logger;

        public DocumentStoreHolder(IOptions<RavenSettings> ravenSettings, ILogger<DocumentStoreHolder> logger)
        {
            this._logger = logger;
            var settings = ravenSettings.Value;
            
            // TODO: Connect to RavenDB            
            this._logger.LogWarning("⚠️  Not connected to Raven document store");
        }

        public IDocumentStore Store { get; }
    }
}