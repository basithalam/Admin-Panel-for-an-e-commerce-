using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Models;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Pages.Invoice
{
    public class InvoiceIndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<InvoiceIndexModel> _logger;

        public InvoiceIndexModel(IOrderService orderService, ILogger<InvoiceIndexModel> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public Order? Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            try
            {
                Order = await _orderService.GetOrderByIdAsync(orderId);

                if (Order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading invoice for order {OrderId}", orderId);
                TempData["ErrorMessage"] = "Unable to load invoice.";
                return RedirectToPage("/Index");
            }
        }
    }
}
