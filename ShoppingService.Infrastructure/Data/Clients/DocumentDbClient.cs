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

        public EitherAsync<Exception, ResourceResponse<Database>> CreateDatabaseAsync(RequestOptions options = null) =>
            match(TryOptionAsync(async () => await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName }, options)),
                Some: database => Right<Exception, ResourceResponse<Database>>(database),
                None: () => Left<Exception, ResourceResponse<Database>>(new Exception("Unknown error occurred - CreateDatabaseAsync")),
                Fail: ex => Left<Exception, ResourceResponse<Database>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, ResourceResponse<Document>> CreateDocumentAsync(T item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), item, options,
                disableAutomaticIdGeneration, cancellationToken
            )),
                Some: document => Right<Exception, ResourceResponse<Document>>(document),
                None: () => Left<Exception, ResourceResponse<Document>>(new Exception("Unknown error occurred - CreateDocumentAsync")),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, IEnumerable<T>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var operation = TryOptionAsync(async () =>
            {
                IDocumentQuery<Document> query = _documentClient.CreateDocumentQuery<Document>(
                    UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName),
                    new FeedOptions { MaxItemCount = itemCountLimit }
                ).AsDocumentQuery();

                var items = new List<T>();
                while (query.HasMoreResults)
                {
                    FeedResponse<T> response = await query.ExecuteNextAsync<T>(cancellationToken);
                    items.AddRange(response);
                }
                return items as IEnumerable<T>;
            });

            return match(operation,
                Some: objects => Right<Exception, IEnumerable<T>>(objects),
                None: () => Left<Exception, IEnumerable<T>>(new Exception("Unknown error occurred - GetDocumentAsync")),
                Fail: ex => Left<Exception, IEnumerable<T>>(ex)
            ).ToAsync();
        }

        public EitherAsync<Exception, ResourceResponse<Document>> GetDocumentByIdAsync(string documentId,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken
            )),
                Some: document => Right<Exception, ResourceResponse<Document>>(document),
                None: () => Left<Exception, ResourceResponse<Document>>(new Exception("Unknown error occurred - GetDocumentByIdAsync")),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, ResourceResponse<Document>> ReplaceDocumentAsync(string documentId, object document,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                document, options, cancellationToken
            )),
                Some: updatedDocument => Right<Exception, ResourceResponse<Document>>(updatedDocument),
                None: () => Left<Exception, ResourceResponse<Document>>(new Exception("Unknown error occurred - ReplaceDocumentAsync")),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, ResourceResponse<Document>> DeleteDocumentAsync(string documentId,
            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            match(TryOptionAsync(async () => await _documentClient.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId),
                options, cancellationToken
            )),
                Some: document => Right<Exception, ResourceResponse<Document>>(document),
                None: () => Left<Exception, ResourceResponse<Document>>(new Exception("Unknown error occurred - DeleteDocumentAsync")),
                Fail: ex => Left<Exception, ResourceResponse<Document>>(ex)
            ).ToAsync();
    }
}
