namespace Ecommerce.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public int LowStockProducts { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

