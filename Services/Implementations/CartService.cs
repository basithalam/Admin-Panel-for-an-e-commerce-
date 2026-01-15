using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;
using System.Text.Json;

namespace EcommerceRazorApp.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartService> _logger;
        private const string CartSessionKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor, ILogger<CartService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        public List<CartItem> GetCart()
        {
            try
            {
                if (Session == null) return new List<CartItem>();

                var cartJson = Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartJson))
                {
                    return new List<CartItem>();
                }

                return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart from session");
                return new List<CartItem>();
            }
        }

        public void AddToCart(CartItem item)
        {
            try
            {
                if (Session == null)
                {
                    _logger.LogWarning("Session is not available");
                    return;
                }

                var cart = GetCart();
                var existingItem = cart.FirstOrDefault(c => c.ProductId == item.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                    _logger.LogInformation("Updated quantity for product {ProductId} to {Quantity}", item.ProductId, existingItem.Quantity);
                }
                else
                {
                    cart.Add(item);
                    _logger.LogInformation("Added new product {ProductId} to cart", item.ProductId);
                }

                SaveCart(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                throw;
            }
        }

        public void RemoveFromCart(int productId)
        {
            try
            {
                if (Session == null) return;

                var cart = GetCart();
                var item = cart.FirstOrDefault(c => c.ProductId == productId);

                if (item != null)
                {
                    cart.Remove(item);
                    _logger.LogInformation("Removed product {ProductId} from cart", productId);
                    SaveCart(cart);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                throw;
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            try
            {
                if (Session == null) return;

                var cart = GetCart();
                var item = cart.FirstOrDefault(c => c.ProductId == productId);

                if (item != null)
                {
                    if (quantity <= 0)
                    {
                        RemoveFromCart(productId);
                    }
                    else
                    {
                        item.Quantity = quantity;
                        _logger.LogInformation("Updated quantity for product {ProductId} to {Quantity}", productId, quantity);
                        SaveCart(cart);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item quantity");
                throw;
            }
        }

        public void ClearCart()
        {
            try
            {
                if (Session == null) return;

                Session.Remove(CartSessionKey);
                _logger.LogInformation("Cart cleared");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                throw;
            }
        }

        public decimal GetCartTotal()
        {
            try
            {
                var cart = GetCart();
                return cart.Sum(item => item.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cart total");
                return 0;
            }
        }

        public int GetCartItemCount()
        {
            try
            {
                var cart = GetCart();
                return cart.Sum(item => item.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count");
                return 0;
            }
        }

        private void SaveCart(List<CartItem> cart)
        {
            try
            {
                if (Session == null) return;

                var cartJson = JsonSerializer.Serialize(cart);
                Session.SetString(CartSessionKey, cartJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving cart to session");
                throw;
            }
        }
    }
}
