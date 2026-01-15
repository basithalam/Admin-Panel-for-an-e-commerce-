using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IProductService productService, ICartService cartService, ILogger<IndexModel> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _logger = logger;
        }

        public List<Product> FeaturedProducts { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                FeaturedProducts = await _productService.GetFeaturedProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading featured products");
                TempData["ErrorMessage"] = "Unable to load products. Please try again later.";
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id)
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
                TempData["SuccessMessage"] = $"{product.Name} added to cart!";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                TempData["ErrorMessage"] = "Unable to add product to cart.";
                return RedirectToPage();
            }
        }
    }
}
