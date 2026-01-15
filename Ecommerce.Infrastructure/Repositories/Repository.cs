using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Application.Interfaces;
using Ecommerce.Infrastructure.Data;

namespace Ecommerce.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context; // Hold DbContext
            _dbSet = _context.Set<T>(); // Cache DbSet for faster access
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync(); // Read-only query
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id); // Find by primary key
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).AsNoTracking().ToListAsync(); // Filtered read-only query
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); // Track new entity
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity); // Mark entity modified
            return Task.CompletedTask;
        }

        public Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity); // Mark entity deleted
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync(); // Persist changes
        }
    }
}
