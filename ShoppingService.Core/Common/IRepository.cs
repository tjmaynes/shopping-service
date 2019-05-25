using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingService.Core.Common
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T newItem);
        Task<T> GetById(Guid id);
        Task<Guid> Remove(Guid id);
    }
}
