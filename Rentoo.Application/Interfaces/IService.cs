using System.Linq.Expressions;

namespace Rentoo.Application.Interfaces;

public interface IService<T> where T : class
{
    Task<T> GetByIdAsync(int? id);
    Task<T> GetByIdAsync(string id);
    Task<T> GetByIdAsync(int? id, params string[] includeProperties);
    //GetByLicenseNumberAsync
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int ? id);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties);
}

