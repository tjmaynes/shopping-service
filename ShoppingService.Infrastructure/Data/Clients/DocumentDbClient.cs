using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace ShoppingService.Infrastructure.Data.Clients
{
    public class DocumentDbClient<T> : IDocumentDbClient<T>
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IDocumentClient _documentClient;

        public DocumentDbClient(string databaseName, string collectionName, IDocumentClient documentClient)
        {
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            _documentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
        }

        public async Task<Database> CreateDatabaseAsync(RequestOptions options = null) =>
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName }, options);

        public async Task<Document> CreateDocumentAsync(T item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false,
            CancellationToken cancellationToken = default(CancellationToken)) =>
                await _documentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), item, options,
                    disableAutomaticIdGeneration, cancellationToken
                );

        public async Task<IEnumerable<T>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IDocumentQuery<Document> query = _documentClient.CreateDocumentQuery<Document>(
                UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName),
                new FeedOptions { MaxItemCount = itemCountLimit }
            ).AsDocumentQuery();

            List<T> items = new List<T>();
            while (query.HasMoreResults)
            {
                FeedResponse<T> response = await query.ExecuteNextAsync<T>(cancellationToken);
                items.AddRange(response);
            }

            return items;
        }

        public async Task<Document> GetDocumentByIdAsync(string documentId, RequestOptions options = null,
             CancellationToken cancellationToken = default(CancellationToken)) =>
             await _documentClient.ReadDocumentAsync(
                 UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                 options, cancellationToken
             );

        public async Task<Document> ReplaceDocumentAsync(string documentId, object document, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            await _documentClient.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), document, options,
                cancellationToken);

        public async Task<Document> DeleteDocumentAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            await _documentClient.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken);
    }
}
