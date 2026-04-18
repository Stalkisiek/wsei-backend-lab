using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryGenericRepository<T>: IGenericRepositoryAsync<T>
    where T: EntityBase
{
    protected Dictionary<Guid, T> _data = new();

    public async Task<T?> FindByIdAsync(Guid id)
    {
        var result = _data.TryGetValue(id, out var value) ? value : null;
        return await Task.FromResult(result);
    }

    async Task<IEnumerable<T>> IGenericRepositoryAsync<T>.FindAllAsync()
    {
        return await Task.FromResult(_data.Values.ToList());
    }

    public Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var total = _data.Count;
        var items = _data.Values.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var result = new PagedResult<T>(items, total, page, pageSize);
        return Task.FromResult(result);
    }

    public Task<T> AddAsync(T entity)
    {
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task RemoveByIdAsync(Guid id)
    {
        _data.Remove(id);
        return Task.CompletedTask;
    }
}