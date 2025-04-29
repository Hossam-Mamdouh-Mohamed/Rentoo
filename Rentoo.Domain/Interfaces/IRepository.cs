using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rentoo.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);


}