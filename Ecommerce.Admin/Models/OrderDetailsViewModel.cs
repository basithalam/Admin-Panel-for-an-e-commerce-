using Ecommerce.Domain.Entities;

namespace Ecommerce.Admin.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new();
        public Payment? Payment { get; set; }
        public List<string> AllowedStatuses { get; set; } = new() { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        public List<string> AllowedPaymentStatuses { get; set; } = new() { "Pending", "Completed", "Failed", "Refunded" };
    }
}
