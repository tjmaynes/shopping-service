using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingService.Core.Cart
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetAllItems();
        Task<CartItem> Add(CartItem newItem);
        Task<CartItem> GetById(Guid id);
        Task<Guid> Remove(Guid id);
    }
}
