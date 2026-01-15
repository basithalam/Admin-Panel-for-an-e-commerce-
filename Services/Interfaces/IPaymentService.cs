using EcommerceRazorApp.Models;

namespace EcommerceRazorApp.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(int orderId, string paymentMethod, decimal amount, string? cardNumber = null);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        bool ValidateVisaCard(string cardNumber);
    }
}
