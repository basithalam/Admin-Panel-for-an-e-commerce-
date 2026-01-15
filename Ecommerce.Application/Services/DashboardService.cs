using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;

        public DashboardService(IRepository<Order> orderRepository, IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Count();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<int> GetTotalProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Count();
        }

        public async Task<int> GetLowStockProductCountAsync(int threshold)
        {
            var products = await _productRepository.FindAsync(p => p.Stock <= threshold);
            return products.Count();
        }

        public async Task<int> GetTodayOrdersAsync()
        {
            var today = DateTime.UtcNow.Date;
            var orders = await _orderRepository.FindAsync(o => o.OrderDate.Date == today);
            return orders.Count();
        }
    }
}

