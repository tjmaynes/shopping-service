using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Data.Clients
{
    public interface IDocumentDbClient<T>
    {
        TryOptionAsync<T> CreateDocumentAsync(T item);
        TryOptionAsync<List<T>> GetDocumentsAsync(int pageNumber = 0, int pageSize = 50);
        TryOptionAsync<T> GetDocumentByIdAsync(Func<T, bool> compareFunc);
        TryOptionAsync<T> ReplaceDocumentAsync(Func<T, bool> compareFunc, T document);
        TryOptionAsync<T> DeleteDocumentAsync(Func<T, bool> compareFunc);
    }
}
