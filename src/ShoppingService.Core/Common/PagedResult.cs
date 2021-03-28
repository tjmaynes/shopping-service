using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Common
{
    public class PagedResult<T> : Record<PagedResult<T>>
    {
        public IEnumerable<T> Items;
        public long TotalCount;
        public long TotalPages;
        public long CurrPage;

        public PagedResult(IEnumerable<T> items, long totalCount, long totalPages, long currPage) {
            Items = items;
            TotalCount = totalCount;
            TotalPages = totalPages;
            CurrPage = currPage;
        }

        public static PagedResult<T> CreateEmptyResult() => new PagedResult<T>(new List<T>(), 0, 0, 1);
    }
}
