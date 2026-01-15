using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Pages.Cart
{
    public class CartIndexModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartIndexModel> _logger;

        public CartIndexModel(ICartService cartService, ILogger<CartIndexModel> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal CartTotal { get; set; }

        public void OnGet()
        {
            try
            {
                CartItems = _cartService.GetCart();
                CartTotal = _cartService.GetCartTotal();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading cart");
                TempData["ErrorMessage"] = "Unable to load cart.";
            }
        }

        public IActionResult OnPostRemove(int productId)
        {
            try
            {
                _cartService.RemoveFromCart(productId);
                TempData["SuccessMessage"] = "Item removed from cart.";
                return RedirectToPage("/Cart/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                TempData["ErrorMessage"] = "Unable to remove item.";
                return RedirectToPage("/Cart/Index");
            }
        }

        public IActionResult OnPostUpdateQuantity(int productId, int quantity)
        {
            try
            {
                if (quantity < 1)
                {
                    TempData["ErrorMessage"] = "Quantity must be at least 1.";
                    return RedirectToPage("/Cart/Index");
                }

                _cartService.UpdateQuantity(productId, quantity);
                TempData["SuccessMessage"] = "Cart updated.";
                return RedirectToPage("/Cart/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity");
                TempData["ErrorMessage"] = "Unable to update quantity.";
                return RedirectToPage("/Cart/Index");
            }
        }
    }
}
