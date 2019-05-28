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
    public interface IDocumentDbClient
    {
        EitherAsync<Exception, ResourceResponse<Database>> CreateDatabaseAsync(RequestOptions options = null);
        EitherAsync<Exception, ResourceResponse<Document>> CreateDocumentAsync(object item, RequestOptions options = null,
            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken));
        EitherAsync<Exception, IEnumerable<object>> GetDocumentsAsync(int itemCountLimit = 200,
            CancellationToken cancellationToken = default(CancellationToken));
        EitherAsync<Exception, ResourceResponse<Document>> GetDocumentByIdAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
        EitherAsync<Exception, ResourceResponse<Document>> ReplaceDocumentAsync(string documentId, object document, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
        EitherAsync<Exception, ResourceResponse<Document>> DeleteDocumentAsync(string documentId, RequestOptions options = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
