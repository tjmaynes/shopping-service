using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingService.Core.Common;

namespace ShoppingService.Core.Cart
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> _repository;

        public CartService(IRepository<CartItem> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CartItem>> GetAllItemsFromCart()
        {
            throw new NotImplementedException();
        }

        public async Task<CartItem> AddItemToCart(CartItem newItem)
        {
            throw new NotImplementedException();
        }

        public async Task<CartItem> GetItemById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> RemoveItemFromCart(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
