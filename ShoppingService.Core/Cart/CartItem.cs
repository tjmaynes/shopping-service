using System;
using ShoppingService.Core.Common;

namespace ShoppingService.Core.Cart
{
    public class CartItem: Entity
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public string Manufacturer { get; private set; }

        public CartItem(Guid id, string name, decimal price, string manufacturer, DateTime createdAt)
        {
            Id = id;
            Name = name;
            Price = price;
            Manufacturer = manufacturer;
            CreatedAt = createdAt;
        }
    }
}
