using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.DataAccess.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? IncludeProperties = null);
        Task<IQueryable<T>> GetAllAsQueryable(Expression<Func<T, bool>>? filter = null, string? IncludeProperties = null);

        Task<T> Get(Expression<Func<T, bool>> filter, string? IncludeProperties = null, bool tracked = false);
        Task Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
