using System;
using Microsoft.EntityFrameworkCore;
using Rentoo.Domain.Interfaces;
using Rentoo.Infrastructure.Data;

namespace Rentoo.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly RentooDbContext _context; // Fixed type to AppDbContext  
    protected readonly DbSet<T> _dbSet;

    public Repository(RentooDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);
}

