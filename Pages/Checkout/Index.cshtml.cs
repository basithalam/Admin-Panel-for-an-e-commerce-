using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRazorApp.Pages.Checkout
{
    public class CheckoutIndexModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<CheckoutIndexModel> _logger;

        public CheckoutIndexModel(
            ICartService cartService,
            IOrderService orderService,
            IPaymentService paymentService,
            ILogger<CheckoutIndexModel> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _paymentService = paymentService;
            _logger = logger;
        }

        [BindProperty]
        public CustomerInfo CustomerInfo { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [BindProperty]
        public string? CardNumber { get; set; }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal CartTotal { get; set; }

        public IActionResult OnGet()
        {
            try
            {
                CartItems = _cartService.GetCart();
                CartTotal = _cartService.GetCartTotal();

                if (!CartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty.";
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout page");
                TempData["ErrorMessage"] = "Unable to load checkout page.";
                return RedirectToPage("/Cart/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                CartItems = _cartService.GetCart();
                CartTotal = _cartService.GetCartTotal();

                if (!CartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty.";
                    return RedirectToPage("/Cart/Index");
                }

                // Validate card number for Visa payment
                if (PaymentMethod == "Visa Card")
                {
                    if (string.IsNullOrWhiteSpace(CardNumber))
                    {
                        ModelState.AddModelError("CardNumber", "Card number is required for Visa payment");
                    }
                    else if (!_paymentService.ValidateVisaCard(CardNumber))
                    {
                        ModelState.AddModelError("CardNumber", "Invalid Visa card number. Must start with 4 and be 13-19 digits");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Create order
                var order = await _orderService.CreateOrderAsync(CustomerInfo, CartItems, CartTotal);

                // Process payment
                var payment = await _paymentService.ProcessPaymentAsync(
                    order.OrderId,
                    PaymentMethod,
                    CartTotal,
                    CardNumber
                );

                // Check payment status
                if (payment.PaymentStatus == "Failed")
                {
                    TempData["ErrorMessage"] = $"Payment failed: {payment.PaymentNotes}";
                    return RedirectToPage("/Checkout/Index");
                }

                // Clear cart
                _cartService.ClearCart();

                // Redirect to invoice
                return RedirectToPage("/Invoice/Index", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout");
                TempData["ErrorMessage"] = "An error occurred while processing your order. Please try again.";
                return RedirectToPage("/Checkout/Index");
            }
        }
    }
}
