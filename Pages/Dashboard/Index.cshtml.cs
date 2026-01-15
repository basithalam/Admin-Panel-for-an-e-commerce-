using Microsoft.AspNetCore.Mvc.RazorPages;
using EcommerceRazorApp.Services.Interfaces;

namespace EcommerceRazorApp.Pages.Dashboard
{
    public class DashboardIndexModel : PageModel
    {
        private readonly ICartService _cartService;

        public DashboardIndexModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public int CartItemCount { get; set; }

        public void OnGet()
        {
            CartItemCount = _cartService.GetCartItemCount();
        }
    }
}

