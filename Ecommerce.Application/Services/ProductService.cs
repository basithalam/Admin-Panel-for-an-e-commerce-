using Microsoft.Extensions.Logging;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository; // Repository for Product
        private readonly ICategoryRepository _categoryRepository; // Repository for Category
        private readonly ILogger<ProductService> _logger; // Logger for diagnostics

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository; // Inject product repository
            _categoryRepository = categoryRepository; // Inject category repository
            _logger = logger; // Inject logger
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Fetching all products with category");
            return (await _productRepository.GetAllWithCategoryAsync()).ToList();
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            _logger.LogInformation("Fetching featured products");
            return (await _productRepository.FindAsync(p => p.IsFeatured)).ToList();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching products for category {CategoryId}", categoryId);
            return (await _productRepository.GetByCategoryAsync(categoryId)).ToList();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Fetching product with id {ProductId}", id);
            return await _productRepository.GetByIdWithCategoryAsync(id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories");
            return (await _categoryRepository.GetAllAsync()).ToList();
        }

        public async Task<List<Product>> GetProductsSortedByPriceAsync(bool ascending = true)
        {
            _logger.LogInformation("Fetching products sorted by price ascending: {Ascending}", ascending);
            var products = await _productRepository.GetAllWithCategoryAsync();
            return ascending
                ? products.OrderBy(p => p.Price).ToList()
                : products.OrderByDescending(p => p.Price).ToList();
        }

        public async Task<List<Product>> GetProductsWithPaginationAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching products page {PageNumber} size {PageSize}", pageNumber, pageSize);
            var products = await _productRepository.GetAllWithCategoryAsync();
            return products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            _logger.LogInformation("Fetching total product count");
            var products = await _productRepository.GetAllAsync();
            return products.Count();
        }

        public async Task<List<Product>> GetProductsByCategoryWithPaginationAsync(int categoryId, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching products for category {CategoryId} page {PageNumber}", categoryId, pageNumber);
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching product count for category {CategoryId}", categoryId);
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Count();
        }
    }
}
