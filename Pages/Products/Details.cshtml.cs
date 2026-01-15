using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ecommerce.Domain.Entities;
using EcommerceRazorApp.Models;
using Ecommerce.Application.Interfaces;
using EcommerceRazorApp.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRazorApp.Pages.Products
{
    public class ProductDetailsModel : PageModel
    {
        private readonly Ecommerce.Application.Interfaces.IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ILogger<ProductDetailsModel> _logger;

        public ProductDetailsModel(Ecommerce.Application.Interfaces.IProductService productService, ICartService cartService, ILogger<ProductDetailsModel> logger)
        {
            _productService = productService;
            _cartService = cartService;
            _logger = logger;
        }

        public Ecommerce.Domain.Entities.Product? Product { get; set; }

        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Product = await _productService.GetProductByIdAsync(id);

                if (Product == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details");
                TempData["ErrorMessage"] = "Unable to load product details.";
                return RedirectToPage("/Products/Index");
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Product = await _productService.GetProductByIdAsync(id);
                    return Page();
                }

                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToPage("/Products/Index");
                }

                if (Quantity > product.Stock)
                {
                    TempData["ErrorMessage"] = $"Only {product.Stock} units available.";
                    return RedirectToPage("/Products/Details", new { id });
                }

                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = Quantity,
                    ImageUrl = product.ImageUrl
                };

                _cartService.AddToCart(cartItem);
                TempData["SuccessMessage"] = $"{Quantity} x {product.Name} added to cart!";

                return RedirectToPage("/Products/Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                TempData["ErrorMessage"] = "Unable to add product to cart.";
                return RedirectToPage(new { id });
            }
        }

        public async Task<IActionResult> OnPostBuyNowAsync(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Product = await _productService.GetProductByIdAsync(id);
                    return Page();
                }

                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToPage("/Products/Index");
                }

                if (Quantity > product.Stock)
                {
                    TempData["ErrorMessage"] = $"Only {product.Stock} units available.";
                    return RedirectToPage("/Products/Details", new { id });
                }

                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = Quantity,
                    ImageUrl = product.ImageUrl
                };

                _cartService.AddToCart(cartItem);
                return RedirectToPage("/Cart/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in buy now");
                TempData["ErrorMessage"] = "Unable to process request.";
                return RedirectToPage(new { id });
            }
        }
    }
}
