using System;
using System.Collections.Generic;
using System.Linq;
using ShoppingService.Core.Common;
using ShoppingService.Core.Cart;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Api.Tests.Data {
    public class CartRepositoryFake : IRepository<CartItem> {
        private readonly IEnumerable<CartItem> _items;

        public CartRepositoryFake() {
            var items = new List<CartItem>();
            items.Add(new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            _items = items;
        }

        public EitherAsync<Exception, IEnumerable<CartItem>> GetAll(int countLimit = 200) =>
            Right<Exception, IEnumerable<CartItem>>(_items).ToAsync();

        public EitherAsync<Exception, CartItem> Add(CartItem newItem) =>
            Right<Exception, CartItem>(newItem).ToAsync();

        public EitherAsync<Exception, CartItem> GetById(Guid id) =>
            Right<Exception, CartItem>(_items.FirstOrDefault()).ToAsync();

        public EitherAsync<Exception, CartItem> Update(CartItem updatedItem) =>
            Right<Exception, CartItem>(updatedItem).ToAsync();

        public EitherAsync<Exception, Guid> Remove(Guid id) =>
            Right<Exception, Guid>(id).ToAsync();
    }
}
