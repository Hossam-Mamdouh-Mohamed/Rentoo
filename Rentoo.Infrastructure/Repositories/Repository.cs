using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rentoo.Domain.Interfaces;
using Rentoo.Infrastructure.Data;

namespace Rentoo.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly RentooDbContext _context; 
    protected readonly DbSet<T> _dbSet;

    public Repository(RentooDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public async Task<T> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includeProperties)
    {
        IQueryable<T> query = _dbSet;
        
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.Where(predicate).ToListAsync();
    }
}

