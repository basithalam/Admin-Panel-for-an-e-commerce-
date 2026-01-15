using Microsoft.EntityFrameworkCore;
using EcommerceRazorApp.Data;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all products");
                return await _context.Products
                    .Include(p => p.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products");
                throw;
            }
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching featured products");
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsFeatured)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching featured products");
                throw;
            }
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation("Fetching products for category {CategoryId}", categoryId);
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products for category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID {ProductId}", id);
                return await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.ProductId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all categories");
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all categories");
                throw;
            }
        }

        public async Task<List<Product>> GetProductsSortedByPriceAsync(bool ascending = true)
        {
            try
            {
                _logger.LogInformation("Fetching products sorted by price (ascending: {Ascending})", ascending);
                var query = _context.Products.Include(p => p.Category);

                return ascending
                    ? await query.OrderBy(p => p.Price).ToListAsync()
                    : await query.OrderByDescending(p => p.Price).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sorted products");
                throw;
            }
        }

        public async Task<List<Product>> GetProductsWithPaginationAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Fetching products - Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
                return await _context.Products
                    .Include(p => p.Category)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated products");
                throw;
            }
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            try
            {
                _logger.LogInformation("Fetching total product count");
                return await _context.Products.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching total product count");
                throw;
            }
        }

        public async Task<List<Product>> GetProductsByCategoryWithPaginationAsync(int categoryId, int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Fetching products for category {CategoryId} - Page {PageNumber}", categoryId, pageNumber);
                return await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated products for category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation("Fetching product count for category {CategoryId}", categoryId);
                return await _context.Products.CountAsync(p => p.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product count for category {CategoryId}", categoryId);
                throw;
            }
        }
    }
}
