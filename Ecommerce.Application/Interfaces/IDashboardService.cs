using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<int> GetTotalOrdersAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalProductsAsync();
        Task<int> GetLowStockProductCountAsync(int threshold);
        Task<int> GetTodayOrdersAsync();
    }
}

