using System;
using System.Linq;
using ShoppingService.Core.Common;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Cart {
    public class CartRepository : IRepository<CartItem>
    {
        private readonly IDatabaseClient<CartItem> _databaseClient;

        public CartRepository(IDatabaseClient<CartItem> databaseClient)
        {
            _databaseClient = databaseClient ?? throw new ArgumentNullException(nameof(databaseClient));
        }

        public EitherAsync<Exception, Option<PagedResult<CartItem>>> GetAll(int pageNumber = 0, int pageSize = 200) =>
            match(_databaseClient.GetItems(pageNumber, pageSize),
                Some: items => {
                    var totalCount = items.Count();
                    var totalPages = (long)Math.Ceiling((double)(totalCount / pageSize));
                    var collection = new PagedResult<CartItem>(items, totalCount, totalPages, pageNumber + 1);
                    return Right<Exception, Option<PagedResult<CartItem>>>(Some(collection));
                },
                None: () => Right<Exception, Option<PagedResult<CartItem>>>(Some(PagedResult<CartItem>.CreateEmptyResult())),
                Fail: ex => Left<Exception, Option<PagedResult<CartItem>>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Add(CartItem newItem) =>
            match(_databaseClient.AddItem(newItem),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(Some(newItem)),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> GetById(Guid id) =>
            match(_databaseClient.GetItemById((item => item.Id == id)),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(None),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Update(CartItem updatedItem) =>
            match(_databaseClient.ReplaceItem(item => item.Id == updatedItem.Id, updatedItem),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(None),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Remove(Guid id) =>
            match(_databaseClient.DeleteItem((item => item.Id == id)),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(None),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();
    }
}
