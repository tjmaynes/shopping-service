using System;
using System.Collections.Generic;
using ShoppingService.Api.Cart;
using ShoppingService.Core.Common;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Api {
    public class DatabaseClient<T>: IDatabaseClient<T> {
        private readonly ApplicationContext _applicationContext;

        public DatabaseClient(ApplicationContext applicationContext) {
            this._applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        public TryOptionAsync<T> AddItem(T item) =>
            TryOptionAsync(async () =>
            {
                await _applicationContext.FindAsync(item);
                return Some(item);
            });
        
        public TryOptionAsync<List<T>> GetItems(int pageNumber = 0, int pageSize = 50) {

        }

        public TryOptionAsync<T> GetItemById(Func<T, bool> compareFunc) {

        }

        public TryOptionAsync<T> ReplaceItem(Func<T, bool> compareFunc, T item) {

        }

        public TryOptionAsync<T> DeleteItem(Func<T, bool> compareFunc) {

        }
    }
}