using Microsoft.EntityFrameworkCore;
using EcommerceRazorApp.Data;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Payment> ProcessPaymentAsync(int orderId, string paymentMethod, decimal amount, string? cardNumber = null)
        {
            try
            {
                _logger.LogInformation("Processing payment for order {OrderId} using {PaymentMethod}", orderId, paymentMethod);

                string paymentStatus;
                string? transactionId = null;
                string? paymentNotes = null;

                if (paymentMethod == "Visa Card")
                {
                    if (string.IsNullOrEmpty(cardNumber))
                    {
                        throw new ArgumentException("Card number is required for Visa Card payment");
                    }

                    // Validate card format
                    if (!ValidateVisaCard(cardNumber))
                    {
                        paymentStatus = "Failed";
                        paymentNotes = "Invalid card format";
                        _logger.LogWarning("Invalid card format for order {OrderId}", orderId);
                    }
                    else
                    {
                        // Check if card starts with 4 (successful payment simulation)
                        if (cardNumber.StartsWith("4"))
                        {
                            paymentStatus = "Completed";
                            transactionId = Guid.NewGuid().ToString();
                            paymentNotes = "Payment processed successfully";
                            _logger.LogInformation("Payment successful for order {OrderId}", orderId);
                        }
                        else
                        {
                            paymentStatus = "Failed";
                            paymentNotes = "Payment declined by bank";
                            _logger.LogWarning("Payment failed for order {OrderId}", orderId);
                        }
                    }
                }
                else if (paymentMethod == "Cash On Delivery")
                {
                    paymentStatus = "Pending";
                    transactionId = Guid.NewGuid().ToString();
                    paymentNotes = "Cash payment will be collected on delivery";
                    _logger.LogInformation("COD payment registered for order {OrderId}", orderId);
                }
                else
                {
                    throw new ArgumentException($"Invalid payment method: {paymentMethod}");
                }

                var payment = new Payment
                {
                    OrderId = orderId,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = paymentStatus,
                    TransactionId = transactionId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    PaymentNotes = paymentNotes
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Payment record created for order {OrderId}", orderId);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            try
            {
                _logger.LogInformation("Fetching payment for order {OrderId}", orderId);
                return await _context.Payments
                    .FirstOrDefaultAsync(p => p.OrderId == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment for order {OrderId}", orderId);
                throw;
            }
        }

        public bool ValidateVisaCard(string cardNumber)
        {
            try
            {
                // Remove spaces and dashes
                cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

                // Check if it's numeric and has valid length (13-19 digits)
                if (!cardNumber.All(char.IsDigit) || cardNumber.Length < 13 || cardNumber.Length > 19)
                {
                    return false;
                }

                // Visa cards start with 4
                if (!cardNumber.StartsWith("4"))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating card number");
                return false;
            }
        }
    }
}
