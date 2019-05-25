using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using ShoppingService.Infrastructure.Data.Clients;

namespace ShoppingService.Infrastructure.Tests.Fixtures
{
    public class DocumentDbClientFixture : IDisposable
    {
        public string DatabaseName { get; } = "some-database-name";
        public string CollectionName { get; } = "some-collection-name";
        public string DocumentId { get; } = "some-document-id";
        public Uri DocumentUri { get; private set; }

        public DocumentDbClientFixture()
        {
            DocumentUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, DocumentId);
        }

        public IDocumentDbClient<object> Initialize(IDocumentClient documentClient)
        {
            return new DocumentDbClient<object>(DatabaseName, CollectionName, documentClient);
        }

        public void Dispose() { }
    }
}
