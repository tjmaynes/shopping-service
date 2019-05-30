using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Clients;
using Newtonsoft.Json;
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

        public EitherAsync<Exception, IEnumerable<CartItem>> GetAll(int countLimit = 200) =>
            match(_dbClient.GetDocumentsAsync(countLimit),
                Right: items => Right<Exception, IEnumerable<CartItem>>(items.Select(item => (CartItem)item)),
                Left: ex => Left<Exception, IEnumerable<CartItem>>(ex)
            ).ToAsync();

        public EitherAsync<Exception, CartItem> Add(CartItem item) =>
            match(_dbClient.CreateDocumentAsync(item),
               Right: document => Right<Exception, CartItem>(ConvertDocumentIntoCartItem(document.Resource)),
               Left: ex => Left<Exception, CartItem>(ex)
            ).ToAsync();

        public EitherAsync<Exception, CartItem> GetById(Guid id) =>
            match(_dbClient.GetDocumentByIdAsync(id.ToString()),
               Right: document => Right<Exception, CartItem>(ConvertDocumentIntoCartItem(document.Resource)),
               Left: ex => Left<Exception, CartItem>(ex)
            ).ToAsync();

        public EitherAsync<Exception, CartItem> Update(CartItem item) =>
            match(_dbClient.ReplaceDocumentAsync(item.Id.ToString(), item),
                Right: document => Right<Exception, CartItem>(ConvertDocumentIntoCartItem(document.Resource)),
                Left: ex => Left<Exception, CartItem>(ex)
            ).ToAsync();

        public EitherAsync<Exception, Guid> Remove(Guid id) =>
            match(_dbClient.DeleteDocumentAsync(id.ToString()),
                Right: _ => Right<Exception, Guid>(id),
                Left: ex => Left<Exception, Guid>(ex)
            ).ToAsync();

        private CartItem ConvertDocumentIntoCartItem(Document document) =>
            JsonConvert.DeserializeObject<CartItem>(document.ToString());
    }
}
