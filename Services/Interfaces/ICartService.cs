using EcommerceRazorApp.Models;

namespace EcommerceRazorApp.Services.Interfaces
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(CartItem item);
        void RemoveFromCart(int productId);
        void UpdateQuantity(int productId, int quantity);
        void ClearCart();
        decimal GetCartTotal();
        int GetCartItemCount();
    }
}
