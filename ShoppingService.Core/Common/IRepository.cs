using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Common
{
    public interface IRepository<T>
    {
        EitherAsync<Exception, IEnumerable<T>> GetAll(int countLimit = 200);
        EitherAsync<Exception, T> Add(T newItem);
        EitherAsync<Exception, T> GetById(Guid id);
        EitherAsync<Exception, T> Update(T updatedItem);
        EitherAsync<Exception, Guid> Remove(Guid id);
    }
}
