using System;
using ShoppingService.Core.Common;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Cart
{
    public class CartItem : Record<CartItem>
    {
        public string Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public string Manufacturer { get; private set; }

        public CartItem(string id, string name, decimal price, string manufacturer, DateTime createdAt)
        {
            Id = id;
            Name = name;
            Price = price;
            Manufacturer = manufacturer;
            CreatedAt = createdAt;
        }
    }
}
