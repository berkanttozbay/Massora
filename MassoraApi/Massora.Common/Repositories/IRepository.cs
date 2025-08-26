using Massora.Domain.Entities;

namespace Massora.Common.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        // Add Operations
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        // Delete Operations
        Task DeleteAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        // Update Operations
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);

        // Get Operations
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetListAsync();
        Task<IEnumerable<T>> GetWhereAsync(Func<T, bool> predicate);

        // Additional Operations
        Task<bool> AnyAsync(Func<T, bool> predicate);
        Task<int> CountAsync(Func<T, bool> predicate);

        IQueryable<T> GetAsQueryable();
    }
} 