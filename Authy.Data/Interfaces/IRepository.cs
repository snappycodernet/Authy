using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Data.Interfaces
{
    public interface IRepository<T, U> where T : class
    {
        T Create(T entity);
        T Update(U id, T entity);
        void Delete(U id);
        void Delete(T entity);
        T FindById(U id);
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> condition);
        IEnumerable<T> FindAll();
    }
}
