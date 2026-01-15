using Microsoft.EntityFrameworkCore;
using EcommerceRazorApp.Data;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(CustomerInfo customerInfo, List<CartItem> cartItems, decimal totalAmount)
        {
            try
            {
                _logger.LogInformation("Creating order for customer {CustomerName}", customerInfo.FullName);

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerName = customerInfo.FullName,
                    CustomerEmail = customerInfo.Email,
                    CustomerPhone = customerInfo.Phone,
                    ShippingAddress = customerInfo.Address,
                    City = customerInfo.City,
                    PostalCode = customerInfo.PostalCode,
                    Country = customerInfo.Country,
                    TotalAmount = totalAmount,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        Price = cartItem.Price,
                        Quantity = cartItem.Quantity
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} created successfully", order.OrderId);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Fetching order {OrderId}", orderId);
                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all orders");
                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payment)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                _logger.LogInformation("Updating order {OrderId} status to {Status}", orderId, status);
                var order = await _context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    return false;
                }

                order.Status = status;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} status updated successfully", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                throw;
            }
        }
    }
}
