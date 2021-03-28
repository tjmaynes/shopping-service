using ShoppingService.Core.Cart;
using FluentValidation;

namespace ShoppingService.Core.Tests.Data {
    public class CartItemValidatorFixture {
        public static AbstractValidator<CartItem> Initialize() => new CartItemValidator();
    }
}
