using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ShoppingService.Infrastructure.Data.Clients
{
    public interface IDocumentDbClient<T>
    {
        Task<Database> CreateDatabaseAsync(RequestOptions options = null);
        Task<Document> CreateDocumentAsync(T item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<T>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<Document> GetDocumentByIdAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<Document> ReplaceDocumentAsync(string documentId, object document, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
        Task<Document> DeleteDocumentAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
