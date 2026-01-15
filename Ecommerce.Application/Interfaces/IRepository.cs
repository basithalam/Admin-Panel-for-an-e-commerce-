using System.Linq.Expressions;

namespace Ecommerce.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Return all entities
        Task<IEnumerable<T>> GetAllAsync();
        // Return entity by key
        Task<T?> GetByIdAsync(object id);
        // Find entities by predicate
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        // Add new entity
        Task AddAsync(T entity);
        // Update existing entity
        Task UpdateAsync(T entity);
        // Remove entity
        Task RemoveAsync(T entity);
        // Save changes
        Task<int> SaveChangesAsync();
    }
}
