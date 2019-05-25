using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Clients;

namespace ShoppingService.Infrastructure.Data.Repositories
{
    public class CartRepository : IRepository<CartItem>
    {
        private readonly IDocumentDbClient<CartItem> _dbClient;

        public CartRepository(IDocumentDbClient<CartItem> dbClient)
        {
            _dbClient = dbClient;
        }

        public async Task<IEnumerable<CartItem>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<CartItem> Add(CartItem newItem)
        {
            throw new NotImplementedException();
        }

        public async Task<CartItem> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> Remove(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
