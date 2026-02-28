namespace CoreApp.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> AddAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task DeleteAsync(int id);
    Task<T?> UpdateAsync(T entity);
}