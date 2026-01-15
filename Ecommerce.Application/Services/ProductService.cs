using Microsoft.Extensions.Logging;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
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

        public async Task<Product> CreateProductAsync(Product product)
        {
            _logger.LogInformation("Creating new product {Name}", product.Name);

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Failed to create product {Name} because category {CategoryId} does not exist", product.Name, product.CategoryId);
                throw new ArgumentException("Invalid category selected", nameof(product.CategoryId));
            }

            if (product.Price < 0)
            {
                _logger.LogWarning("Failed to create product {Name} because price {Price} is negative", product.Name, product.Price);
                throw new ArgumentException("Price must be non-negative", nameof(product.Price));
            }

            if (product.Stock < 0)
            {
                _logger.LogWarning("Failed to create product {Name} because stock {Stock} is negative", product.Name, product.Stock);
                throw new ArgumentException("Stock must be non-negative", nameof(product.Stock));
            }

            await _productRepository.AddAsync(product);
            var affected = await _productRepository.SaveChangesAsync();

            if (affected <= 0)
            {
                _logger.LogError("SaveChangesAsync returned {Affected} while creating product {Name}", affected, product.Name);
                throw new InvalidOperationException("Product could not be saved");
            }

            _logger.LogInformation("Product {Name} created with id {ProductId}", product.Name, product.ProductId);
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating product {ProductId}", product.ProductId);

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Failed to update product {ProductId} because category {CategoryId} does not exist", product.ProductId, product.CategoryId);
                throw new ArgumentException("Invalid category selected", nameof(product.CategoryId));
            }

            if (product.Price < 0)
            {
                _logger.LogWarning("Failed to update product {ProductId} because price {Price} is negative", product.ProductId, product.Price);
                throw new ArgumentException("Price must be non-negative", nameof(product.Price));
            }

            if (product.Stock < 0)
            {
                _logger.LogWarning("Failed to update product {ProductId} because stock {Stock} is negative", product.ProductId, product.Stock);
                throw new ArgumentException("Stock must be non-negative", nameof(product.Stock));
            }

            await _productRepository.UpdateAsync(product);
            var affected = await _productRepository.SaveChangesAsync();

            if (affected <= 0)
            {
                _logger.LogError("SaveChangesAsync returned {Affected} while updating product {ProductId}", affected, product.ProductId);
                throw new InvalidOperationException("Product could not be updated");
            }

            _logger.LogInformation("Product {ProductId} updated", product.ProductId);
        }

        public async Task DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Delete requested for non-existing product {ProductId}", id);
                return;
            }

            await _productRepository.RemoveAsync(product);
            var affected = await _productRepository.SaveChangesAsync();

            if (affected <= 0)
            {
                _logger.LogError("SaveChangesAsync returned {Affected} while deleting product {ProductId}", affected, id);
                throw new InvalidOperationException("Product could not be deleted");
            }

            _logger.LogInformation("Product {ProductId} deleted", id);
        }
    }
}
