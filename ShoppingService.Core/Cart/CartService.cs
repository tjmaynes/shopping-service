using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ShoppingService.Core.Common;
using FluentValidation;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Cart
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> _repository;
        private readonly AbstractValidator<CartItem> _validator;

        public CartService(IRepository<CartItem> repository, AbstractValidator<CartItem> validator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public EitherAsync<ServiceError, IEnumerable<CartItem>> GetItemsFromCart() =>
            match(_repository.GetAll(),
                Right: items => Right<ServiceError, IEnumerable<CartItem>>(items),
                Left: ex => Left<ServiceError, IEnumerable<CartItem>>(ServiceError.CreateWithSingleMessage(ex.Message, 500))
            ).ToAsync();

        public EitherAsync<ServiceError, CartItem> AddItemToCart(CartItem newItem)
        {
            var result = _validator.Validate(newItem);
            if (result.IsValid) {
                return match(_repository.Add(newItem),
                     Right: item => Right<ServiceError, CartItem>(item),
                     Left: ex => Left<ServiceError, CartItem>(ServiceError.CreateWithSingleMessage(ex.Message, 500))
                 ).ToAsync();
            } else {
                return Left<ServiceError, CartItem>(
                    new ServiceError(result.Errors.Select(error => error.ErrorMessage), 422)
                ).ToAsync();
            }
        }

        public EitherAsync<ServiceError, CartItem> GetItemById(Guid id) =>
            match(_repository.GetById(id),
                Right: item => Right<ServiceError, CartItem>(item),
                Left: ex => Left<ServiceError, CartItem>(ServiceError.CreateWithSingleMessage(ex.Message, 500))
            ).ToAsync();

        public EitherAsync<ServiceError, CartItem> UpdateItemInCart(CartItem updatedItem)
        {
            var result = _validator.Validate(updatedItem);
            if (result.IsValid) {
                return match(_repository.Update(updatedItem),
                     Right: item => Right<ServiceError, CartItem>(item),
                     Left: ex => Left<ServiceError, CartItem>(ServiceError.CreateWithSingleMessage(ex.Message, 500))
                 ).ToAsync();
            } else {
                return Left<ServiceError, CartItem>(
                    new ServiceError(result.Errors.Select(error => error.ErrorMessage), 422)
                ).ToAsync();
            }
        }

        public EitherAsync<ServiceError, Guid> RemoveItemFromCart(Guid id) =>
            match(_repository.Remove(id),
                Right: removedId => Right<ServiceError, Guid>(removedId),
                Left: ex => Left<ServiceError, Guid>(ServiceError.CreateWithSingleMessage(ex.Message, 500))
            ).ToAsync();
    }
}
