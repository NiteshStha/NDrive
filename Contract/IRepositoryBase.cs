using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> FindById(int id);
        Task<T> FindById(Expression<Func<T, bool>> expression);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}