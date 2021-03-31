using System;
using System.Collections.Generic;
using LanguageExt;

namespace ShoppingService.Core.Common
{
    public interface IDatabaseClient<T>
    {
        TryOptionAsync<T> AddItem(T item);
        TryOptionAsync<List<T>> GetItems(int pageNumber = 0, int pageSize = 50);
        TryOptionAsync<T> GetItemById(Func<T, bool> compareFunc);
        TryOptionAsync<T> ReplaceItem(Func<T, bool> compareFunc, T item);
        TryOptionAsync<T> DeleteItem(Func<T, bool> compareFunc);
    }
}