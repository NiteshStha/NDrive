using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contract;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly RepositoryContext _repositoryContext;

        protected RepositoryBase(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<T>> FindAll() => await _repositoryContext.Set<T>().AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> expression) =>
            await _repositoryContext.Set<T>().Where(expression).AsNoTracking().ToListAsync();

        public async Task<T> FindById(int id) => await _repositoryContext.Set<T>().FindAsync(id);

        public async Task<T> FindById(Expression<Func<T, bool>> expression) =>
            await _repositoryContext.Set<T>().SingleOrDefaultAsync(expression);

        public async Task Create(T entity) => await _repositoryContext.Set<T>().AddAsync(entity);

        public void Update(T entity) => _repositoryContext.Set<T>().Update(entity);

        public void Delete(T entity) => _repositoryContext.Set<T>().Remove(entity);
    }
}