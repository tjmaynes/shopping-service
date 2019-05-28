using System;
using ShoppingService.Core.Common;
using ShoppingService.Core.Cart;
using ShoppingService.Api.Services;

namespace ShoppingService.Api.Tests.Data
{
    public class CartServiceFixture
    {
        public ICartService Initialize(IRepository<CartItem> repository) {
            var validator = new CartItemValidator();
            return new CartService(repository, validator);
        }
    }
}
