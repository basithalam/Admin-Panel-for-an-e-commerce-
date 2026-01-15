using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface IProductService
    {
        // Get all products with category
        Task<List<Product>> GetAllProductsAsync();
        // Get featured products
        Task<List<Product>> GetFeaturedProductsAsync();
        // Get products by category id
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        // Get product by id with category
        Task<Product?> GetProductByIdAsync(int id);
        // Get all categories
        Task<List<Category>> GetAllCategoriesAsync();
        // Get products sorted by price
        Task<List<Product>> GetProductsSortedByPriceAsync(bool ascending = true);
        // Get paged products
        Task<List<Product>> GetProductsWithPaginationAsync(int pageNumber, int pageSize);
        // Get total product count
        Task<int> GetTotalProductCountAsync();
        // Get paged products by category
        Task<List<Product>> GetProductsByCategoryWithPaginationAsync(int categoryId, int pageNumber, int pageSize);
        // Get product count by category
        Task<int> GetProductCountByCategoryAsync(int categoryId);
    }
}
