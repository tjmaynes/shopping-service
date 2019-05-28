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
        private readonly IDocumentDbClient _dbClient;

        public CartRepository(IDocumentDbClient dbClient)
        {
            _dbClient = dbClient ?? throw new ArgumentNullException(nameof(dbClient));
        }

        public EitherAsync<string, IEnumerable<CartItem>> GetAll(int countLimit = 200) =>
            match(_dbClient.GetDocumentsAsync(countLimit),
                Some: items => Right<string, IEnumerable<CartItem>>(items.Select(item => (CartItem)item)),
                None: ()    => Left<string, IEnumerable<CartItem>>("No items were found."),
                Fail: ex    => Left<string, IEnumerable<CartItem>>(ex.Message)
            ).ToAsync();

        public EitherAsync<string, CartItem> Add(CartItem item) =>
            match(_dbClient.CreateDocumentAsync(item),
               Some: document => Right<string, CartItem>(ConvertDocumentIntoCartItem(document.Resource)),
               None: ()       => Right<string, CartItem>(item),
               Fail: ex       => Left<string, CartItem>(ex.Message)
            ).ToAsync();

        public EitherAsync<string, CartItem> GetById(Guid id) =>
            match(_dbClient.GetDocumentByIdAsync(id.ToString()),
               Some: document => Right<string, CartItem>(ConvertDocumentIntoCartItem(document.Resource)),
               None: ()       => Left<string, CartItem>("Item not found."),
               Fail: ex       => Left<string, CartItem>(ex.Message)
            ).ToAsync();

        public EitherAsync<string, CartItem> Update(CartItem item) =>
            match(_dbClient.ReplaceDocumentAsync(item.Id.ToString(), item),
                Some: document => Right<string, CartItem>(ConvertDocumentIntoCartItem(document.Resource)),
                None: ()       => Right<string, CartItem>(item),
                Fail: ex       => Left<string, CartItem>(ex.Message)
            ).ToAsync();

        public EitherAsync<string, Guid> Remove(Guid id) =>
            match(_dbClient.DeleteDocumentAsync(id.ToString()),
                Some: _  => Right<string, Guid>(id),
                None: () => Left<string, Guid>("Item not found."),
                Fail: ex => Left<string, Guid>(ex.Message)
            ).ToAsync();
        private CartItem ConvertDocumentIntoCartItem(Document document) =>
            JsonConvert.DeserializeObject<CartItem>(document.ToString());
    }
}
