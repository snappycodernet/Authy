using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Data.Interfaces
{
    public interface IAsyncRepository<T, U> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(U id, T entity);
        Task DeleteAsync(U id);
        Task DeleteAsync(T entity);
        Task<T> FindByIdAsync(U id);
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> condition);
        Task<IEnumerable<T>> FindAllAsync();
    }
}
