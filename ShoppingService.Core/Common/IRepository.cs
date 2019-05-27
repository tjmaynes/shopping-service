using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Common
{
    public interface IRepository<T>
    {
        EitherAsync<string, IEnumerable<T>> GetAll(int countLimit = 200);
        EitherAsync<string, T> Add(T newItem);
        EitherAsync<string, T> GetById(Guid id);
        EitherAsync<string, T> Update(T updatedItem);
        EitherAsync<string, Guid> Remove(Guid id);
    }
}
