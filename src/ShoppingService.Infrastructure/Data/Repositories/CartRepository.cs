using System;
using System.Linq;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Clients;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Data.Repositories
{
    public class CartRepository : IRepository<CartItem>
    {
        private readonly IDocumentDbClient<CartItem> _dbClient;

        public CartRepository(IDocumentDbClient<CartItem> dbClient)
        {
            _dbClient = dbClient ?? throw new ArgumentNullException(nameof(dbClient));
        }

        public EitherAsync<Exception, Option<PagedResult<CartItem>>> GetAll(
            int pageNumber = 0, int pageSize = 200
        ) =>
            match(_dbClient.GetDocumentsAsync(pageNumber, pageSize),
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
            match(_dbClient.CreateDocumentAsync(newItem),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(Some(newItem)),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> GetById(string id) =>
            match(_dbClient.GetDocumentByIdAsync((item => item.Id == id)),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(None),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Update(CartItem updatedItem) =>
            match(_dbClient.ReplaceDocumentAsync((item => item.Id == updatedItem.Id), updatedItem),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(None),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Remove(string id) =>
            match(_dbClient.DeleteDocumentAsync((item => item.Id == id)),
                Some: item => Right<Exception, Option<CartItem>>(Some(item)),
                None: () => Right<Exception, Option<CartItem>>(None),
                Fail: ex => Left<Exception, Option<CartItem>>(ex)
            ).ToAsync();
    }
}
