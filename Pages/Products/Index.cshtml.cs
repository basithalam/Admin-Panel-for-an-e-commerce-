using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Pages.Products
{
    public class ProductsIndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<ProductsIndexModel> _logger;

        private const int PageSize = 6;

        public ProductsIndexModel(IProductService productService, ICartService cartService, ILogger<ProductsIndexModel> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _logger = logger;
        }

        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
        public int? SelectedCategoryId { get; set; }
        public string? SortBy { get; set; }

        public async Task OnGetAsync(int? categoryId, string? sortBy, int page = 1)
        {
            try
            {
                CurrentPage = page;
                SelectedCategoryId = categoryId;
                SortBy = sortBy;

                Categories = await _productService.GetAllCategoriesAsync();

                if (categoryId.HasValue)
                {
                    TotalProducts = await _productService.GetProductCountByCategoryAsync(categoryId.Value);
                    Products = await _productService.GetProductsByCategoryWithPaginationAsync(categoryId.Value, CurrentPage, PageSize);
                }
                else
                {
                    TotalProducts = await _productService.GetTotalProductCountAsync();
                    Products = await _productService.GetProductsWithPaginationAsync(CurrentPage, PageSize);
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    Products = sortBy switch
                    {
                        "price_asc" => Products.OrderBy(p => p.Price).ToList(),
                        "price_desc" => Products.OrderByDescending(p => p.Price).ToList(),
                        _ => Products
                    };
                }

                TotalPages = (int)Math.Ceiling(TotalProducts / (double)PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["ErrorMessage"] = "Unable to load products. Please try again later.";
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id, int? categoryId, string? sortBy, int page = 1)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToPage(new { categoryId, sortBy, page });
                }

                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                };

                _cartService.AddToCart(cartItem);
                TempData["SuccessMessage"] = $"{product.Name} added to cart!";

                return RedirectToPage(new { categoryId, sortBy, page });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                TempData["ErrorMessage"] = "Unable to add product to cart.";
                return RedirectToPage(new { categoryId, sortBy, page });
            }
        }

        public async Task<IActionResult> OnPostBuyNowAsync(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToPage();
                }

                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                };

                _cartService.AddToCart(cartItem);
                return RedirectToPage("/Cart/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in buy now");
                TempData["ErrorMessage"] = "Unable to process request.";
                return RedirectToPage();
            }
        }
    }
}
