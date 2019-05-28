using ShoppingService.Core.Cart;
using ShoppingService.Api.Services;
using FluentValidation;

namespace ShoppingService.Api.Tests.Data {
    public class CartItemValidatorFixture {
        public static AbstractValidator<CartItem> Initialize() => new CartItemValidator();
    }
}
