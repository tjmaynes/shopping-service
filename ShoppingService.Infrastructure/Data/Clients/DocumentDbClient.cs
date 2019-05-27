using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

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

        public TryOptionAsync<ResourceResponse<Database>> CreateDatabaseAsync(RequestOptions options = null) =>
            TryOptionAsync(async () => await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName }, options));

        public TryOptionAsync<ResourceResponse<Document>> CreateDocumentAsync(T item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken)) =>
            TryOptionAsync(async () => await _documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), item, options,
                disableAutomaticIdGeneration, cancellationToken
            ));

        public TryOptionAsync<IEnumerable<T>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            TryOptionAsync(async () => {
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
                return items as IEnumerable<T>;
            });

        public TryOptionAsync<ResourceResponse<Document>> GetDocumentByIdAsync(string documentId,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            TryOptionAsync(async () => await _documentClient.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken
            ));

        public TryOptionAsync<ResourceResponse<Document>> ReplaceDocumentAsync(string documentId, object document,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            TryOptionAsync(async () => await _documentClient.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                document, options, cancellationToken
            ));

        public TryOptionAsync<ResourceResponse<Document>> DeleteDocumentAsync(string documentId,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            TryOptionAsync(async () => await _documentClient.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken
            ));
    }
}
