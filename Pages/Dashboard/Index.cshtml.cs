using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Services.Interfaces;
using Ecommerce.Application.Interfaces;

namespace EcommerceRazorApp.Pages.Dashboard
{
    public class DashboardIndexModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IDashboardService _dashboardService;

        public DashboardIndexModel(ICartService cartService, IDashboardService dashboardService)
        {
            _cartService = cartService;
            _dashboardService = dashboardService;
        }

        public int CartItemCount { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProductCount { get; set; }
        public int TodayOrders { get; set; }

        public async Task OnGetAsync()
        {
            CartItemCount = _cartService.GetCartItemCount();
            TotalOrders = await _dashboardService.GetTotalOrdersAsync();
            TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            TotalProducts = await _dashboardService.GetTotalProductsAsync();
            LowStockProductCount = await _dashboardService.GetLowStockProductCountAsync(5);
            TodayOrders = await _dashboardService.GetTodayOrdersAsync();
        }
    }
}
