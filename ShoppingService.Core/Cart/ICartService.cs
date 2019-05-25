using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingService.Core.Common;

namespace ShoppingService.Core.Cart {
    public interface ICartService {
        Task<IEnumerable<CartItem>> GetAllItemsFromCart();
        Task<CartItem> AddItemToCart(CartItem newItem);
        Task<CartItem> GetItemById(Guid id);
        Task<Guid> RemoveItemFromCart(Guid id);
    }
}
