using System;
using LanguageExt;

namespace ShoppingService.Core.Common
{
    public interface IRepository<T>
    {
        EitherAsync<Exception, Option<PagedResult<T>>> GetAll(
            int pageNumber = 0, int pageSize = 200
        );
        EitherAsync<Exception, Option<T>> Add(T newItem);
        EitherAsync<Exception, Option<T>> GetById(Guid id);
        EitherAsync<Exception, Option<T>> Update(T updatedItem);
        EitherAsync<Exception, Option<T>> Remove(Guid id);
    }
}
