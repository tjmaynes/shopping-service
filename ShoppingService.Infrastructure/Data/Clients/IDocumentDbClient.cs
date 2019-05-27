using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Data.Clients
{
    public interface IDocumentDbClient<T>
    {
        TryOptionAsync<ResourceResponse<Database>> CreateDatabaseAsync(RequestOptions options = null);
        TryOptionAsync<ResourceResponse<Document>> CreateDocumentAsync(T item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken));
        TryOptionAsync<IEnumerable<T>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken));
        TryOptionAsync<ResourceResponse<Document>> GetDocumentByIdAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
        TryOptionAsync<ResourceResponse<Document>> ReplaceDocumentAsync(string documentId, object document, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
        TryOptionAsync<ResourceResponse<Document>> DeleteDocumentAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
