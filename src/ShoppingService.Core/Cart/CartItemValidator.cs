using FluentValidation;

namespace ShoppingService.Core.Cart
{
    public class CartItemValidator : AbstractValidator<CartItem>
    {
        public CartItemValidator()
        {
            RuleFor(cartItem => cartItem.Id).NotNull();
            RuleFor(cartItem => cartItem.Name).NotNull();
            RuleFor(cartItem => cartItem.Price).NotNull().GreaterThan(0);
            RuleFor(cartItem => cartItem.Manufacturer).NotNull();
        }
    }
}
