using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingService.Core.Common;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Cart {
    public interface ICartService {
        EitherAsync<ServiceError, PagedResult<CartItem>> GetItemsFromCart(
            int currentPage = 0, int pageSize = 40);
        EitherAsync<ServiceError, CartItem> AddItemToCart(CartItem newItem);
        EitherAsync<ServiceError, CartItem> GetItemById(string id);
        EitherAsync<ServiceError, CartItem> UpdateItemInCart(CartItem updatedItem);
        EitherAsync<ServiceError, string> RemoveItemFromCart(string id);
    }
}
