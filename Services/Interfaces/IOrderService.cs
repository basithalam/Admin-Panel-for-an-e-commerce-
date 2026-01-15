using EcommerceRazorApp.Models;

namespace EcommerceRazorApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CustomerInfo customerInfo, List<CartItem> cartItems, decimal totalAmount);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}
