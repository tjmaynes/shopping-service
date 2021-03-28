using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ShoppingService.Core.Common;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Cart
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartItem> Repository;
        private readonly AbstractValidator<CartItem> Validator;

        public CartService(IRepository<CartItem> repository, AbstractValidator<CartItem> validator)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public EitherAsync<ServiceError, PagedResult<CartItem>> GetItemsFromCart(
            int currentPage = 0, int pageSize = 40
        ) =>
            match(Repository.GetAll(currentPage, pageSize),
                Right: option => match(option,
                    Some: items => Right<ServiceError, PagedResult<CartItem>>(items),
                    None: () => Right<ServiceError, PagedResult<CartItem>>(PagedResult<CartItem>.CreateEmptyResult())),
                Left: ex => Left<ServiceError, PagedResult<CartItem>>(new ServiceError(ex.Message, ServiceErrorCode.UnknownException))
            ).ToAsync();

        public EitherAsync<ServiceError, CartItem> AddItemToCart(CartItem newItem)
        {
            var result = Validator.Validate(newItem);
            if (result.IsValid) {
                return match(Repository.Add(newItem),
                     Right: option => match(option,
                        Some: item => Right<ServiceError, CartItem>(item),
                        None: () => Right<ServiceError, CartItem>(newItem)),
                     Left: ex => Left<ServiceError, CartItem>(new ServiceError(ex.Message, ServiceErrorCode.UnknownException))
                 ).ToAsync();
            } else {
                return Left<ServiceError, CartItem>(
                    new ServiceError(
                        ConvertValidationErrorsToCSVFormattedString(result.Errors),
                        ServiceErrorCode.InvalidItem
                   )
                ).ToAsync();
            }
        }

        public EitherAsync<ServiceError, CartItem> GetItemById(string id) =>
            match(Repository.GetById(id),
                Right: option => match(option,
                    Some: item => Right<ServiceError, CartItem>(item),
                    None: () => Left<ServiceError, CartItem>(new ServiceError("Item not found!", ServiceErrorCode.ItemNotFound))),
                Left: ex => Left<ServiceError, CartItem>(new ServiceError(ex.Message, ServiceErrorCode.UnknownException))
            ).ToAsync();

        public EitherAsync<ServiceError, CartItem> UpdateItemInCart(CartItem updatedItem)
        {
            var result = Validator.Validate(updatedItem);
            if (result.IsValid) {
                return match(Repository.Update(updatedItem),
                     Right: option => match(option,
                        Some: item => Right<ServiceError, CartItem>(item),
                        None: () => Right<ServiceError, CartItem>(updatedItem)),
                     Left: ex => Left<ServiceError, CartItem>(new ServiceError(ex.Message, ServiceErrorCode.UnknownException))
                 ).ToAsync();
            } else {
                return Left<ServiceError, CartItem>(
                    new ServiceError(
                        ConvertValidationErrorsToCSVFormattedString(result.Errors),
                        ServiceErrorCode.InvalidItem
                    )
                ).ToAsync();
            }
        }

        public EitherAsync<ServiceError, string> RemoveItemFromCart(string id) =>
            match(Repository.Remove(id),
                Right: option => match(option,
                    Some: item => Right<ServiceError, string>(item.Id),
                    None: () => Right<ServiceError, string>(id)),
                Left: ex => Left<ServiceError, string>(new ServiceError(ex.Message, ServiceErrorCode.UnknownException))
            ).ToAsync();

        private string ConvertValidationErrorsToCSVFormattedString(IEnumerable<ValidationFailure> errors) {
            return String.Join(", ", errors.Select(error => error.ErrorMessage));
        }
    }
}
