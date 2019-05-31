using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Common
{
    public interface IRepository<T>
    {
        EitherAsync<Exception, Option<PagedResult<T>>> GetAll(
            int pageNumber = 0, int pageSize = 200
        );
        EitherAsync<Exception, Option<T>> Add(T newItem);
        EitherAsync<Exception, Option<T>> GetById(string id);
        EitherAsync<Exception, Option<T>> Update(T updatedItem);
        EitherAsync<Exception, Option<T>> Remove(string id);
    }
}
