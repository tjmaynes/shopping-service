using System;
using System.Collections.Generic;
using System.Linq;
using ShoppingService.Core.Common;
using ShoppingService.Core.Cart;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Tests.Data {
    public class CartRepositoryFake : IRepository<CartItem> {
        private readonly IEnumerable<CartItem> _items;

        public CartRepositoryFake() {
            var items = new List<CartItem>();
            items.Add(new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            _items = items;
        }

        public EitherAsync<Exception, Option<PagedResult<CartItem>>> GetAll(
            int pageNumber = 0, int pageSize = 200
        ) =>
            Right<Exception, Option<PagedResult<CartItem>>>(Some(new PagedResult<CartItem>(
                _items, 1, 1, 1
            ))).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Add(CartItem newItem) =>
            Right<Exception, Option<CartItem>>(Some(newItem)).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> GetById(string id) =>
            Right<Exception, Option<CartItem>>(Some(_items.FirstOrDefault())).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Update(CartItem updatedItem) =>
            Right<Exception, Option<CartItem>>(Some(updatedItem)).ToAsync();

        public EitherAsync<Exception, Option<CartItem>> Remove(string id) =>
            Right<Exception, Option<CartItem>>(Some(_items.First())).ToAsync();
    }
}
