using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        // Return all products including Category
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        // Return single product including Category
        Task<Product?> GetByIdWithCategoryAsync(int id);
        // Return products by category id including Category
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    }
}
