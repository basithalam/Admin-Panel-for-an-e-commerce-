using EcommerceRazorApp.Models;

namespace EcommerceRazorApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetFeaturedProductsAsync();
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Product>> GetProductsSortedByPriceAsync(bool ascending = true);
        Task<List<Product>> GetProductsWithPaginationAsync(int pageNumber, int pageSize);
        Task<int> GetTotalProductCountAsync();
        Task<List<Product>> GetProductsByCategoryWithPaginationAsync(int categoryId, int pageNumber, int pageSize);
        Task<int> GetProductCountByCategoryAsync(int categoryId);
    }
}
