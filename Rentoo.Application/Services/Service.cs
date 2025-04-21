﻿using Rentoo.Application.Interfaces;
using Rentoo.Domain.Interfaces;

namespace Rentoo.Application.Services;

public class Service<T> : IService<T> where T : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<T> _repository;

    public Service(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.Repository<T>();
    }

    public async Task<T> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public async Task<T> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _repository.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            _repository.Remove(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
