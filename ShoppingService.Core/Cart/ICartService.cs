using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingService.Core.Common;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Cart {
    public interface ICartService {
        EitherAsync<ServiceError, IEnumerable<CartItem>> GetItemsFromCart();
        EitherAsync<ServiceError, CartItem> AddItemToCart(CartItem newItem);
        EitherAsync<ServiceError, CartItem> GetItemById(Guid id);
        EitherAsync<ServiceError, CartItem> UpdateItemInCart(CartItem updatedItem);
        EitherAsync<ServiceError, Guid> RemoveItemFromCart(Guid id);
    }
}
