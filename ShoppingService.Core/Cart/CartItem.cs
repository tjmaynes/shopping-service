using System;
using ShoppingService.Core.Common;

namespace ShoppingService.Core.Cart
{
    public class CartItem : Entity, IEquatable<CartItem>
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

        public override int GetHashCode() => (Id.GetHashCode() ^ CreatedAt.GetHashCode());

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return Equals((CartItem)obj);
        }

        public bool Equals(CartItem other) => (
            Id == other.Id && CreatedAt == other.CreatedAt &&
            Name == other.Name && Price == other.Price &&
            Manufacturer == other.Manufacturer
        );

        public static bool operator ==(CartItem cartItem1, CartItem cartItem2) => cartItem1.Equals(cartItem2);
        public static bool operator !=(CartItem cartItem1, CartItem cartItem2) => !cartItem1.Equals(cartItem2);
    }
}
