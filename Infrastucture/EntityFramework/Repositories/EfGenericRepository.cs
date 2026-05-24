using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.EntityFramework.Repositories;

public class EfGenericRepository<T> : IGenericRepositoryAsync<T> where T : EntityBase
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _set;

    public EfGenericRepository(DbContext context, DbSet<T> set)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _set = set ?? throw new ArgumentNullException(nameof(set));
    }

    public async Task<T?> FindByIdAsync(Guid id)
    {
        return await _set.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAllAsync()
    {
        return await _set.AsNoTracking().ToListAsync();
    }

    public async Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var items = await _set
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var total = await _set.CountAsync();

        var result = new PagedResult<T>(items, total, page, pageSize);
        return result;
    }

    public async Task<T> AddAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        var entry = await _set.AddAsync(entity);
        return entry.Entity;
    }

    public Task<T> UpdateAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        var entry = _set.Update(entity);
        return Task.FromResult(entry.Entity);
    }

    public async Task RemoveByIdAsync(Guid id)
    {
        var entity = await _set.FindAsync(id);
        if (entity == null) return;
        _set.Remove(entity);
    }
}

