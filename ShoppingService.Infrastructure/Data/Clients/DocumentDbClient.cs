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
    public class DocumentDbClient : IDocumentDbClient
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

        public EitherAsync<Exception, ResourceResponse<Database>> CreateDatabaseAsync(RequestOptions options = null) =>
            match(TryOptionAsync(async () => await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName }, options)),
                Some: database => Right<Exception, ResourceResponse<Database>>(database),
                None: () => Left<Exception, ResourceResponse<Database>>(new KeyNotFoundException()),
                Fail: ex => Left<Exception, ResourceResponse<Database>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, ResourceResponse<Document>> CreateDocumentAsync(object item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), item, options,
                disableAutomaticIdGeneration, cancellationToken
            )),
                Some: document => Right<Exception, ResourceResponse<Document>>(document),
                None: () => Left<Exception, ResourceResponse<Document>>(new KeyNotFoundException()),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, IEnumerable<object>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var operation = TryOptionAsync(async () =>
            {
                IDocumentQuery<Document> query = _documentClient.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName),
                    new FeedOptions { MaxItemCount = itemCountLimit }
                ).AsDocumentQuery();

                var items = new List<object>();
                while (query.HasMoreResults)
                {
                    FeedResponse<object> response = await query.ExecuteNextAsync<object>(cancellationToken);
                    items.AddRange(response);
                }
                return items as IEnumerable<object>;
            });

            return match(operation,
                Some: objects => Right<Exception, IEnumerable<object>>(objects),
                None: () => Left<Exception, IEnumerable<object>>(new KeyNotFoundException()),
                Fail: ex => Left<Exception, IEnumerable<object>>(ex)
            ).ToAsync();
        }

        public EitherAsync<Exception, ResourceResponse<Document>> GetDocumentByIdAsync(string documentId,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken
            )),
                Some: document => Right<Exception, ResourceResponse<Document>>(document),
                None: () => Left<Exception, ResourceResponse<Document>>(new KeyNotFoundException()),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, ResourceResponse<Document>> ReplaceDocumentAsync(string documentId, object document,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                document, options, cancellationToken
            )),
                Some: updatedDocument => Right<Exception, ResourceResponse<Document>>(updatedDocument),
                None: () => Left<Exception, ResourceResponse<Document>>(new KeyNotFoundException()),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, ResourceResponse<Document>> DeleteDocumentAsync(string documentId,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken
            )),
                Some: document => Right<Exception, ResourceResponse<Document>>(document),
                None: () => Left<Exception, ResourceResponse<Document>>(new KeyNotFoundException()),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();
    }
}
