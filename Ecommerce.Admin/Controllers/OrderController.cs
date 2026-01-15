using Microsoft.AspNetCore.Mvc;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Admin.Models;

namespace Ecommerce.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Payment> _paymentRepository;

        public OrderController(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Payment> paymentRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            var sorted = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(100)
                .ToList();
            return View(sorted);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            var items = (await _orderItemRepository.FindAsync(i => i.OrderId == id)).ToList();
            var payment = (await _paymentRepository.FindAsync(p => p.OrderId == id)).FirstOrDefault();

            var vm = new OrderDetailsViewModel
            {
                Order = order,
                Items = items,
                Payment = payment
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            var allowed = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!allowed.Contains(status))
            {
                ModelState.AddModelError("Status", "Invalid status");
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Status = status;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
            TempData["Message"] = $"Order status updated to {status}";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePaymentStatus(int orderId, string paymentStatus)
        {
            var payment = (await _paymentRepository.FindAsync(p => p.OrderId == orderId)).FirstOrDefault();
            if (payment == null)
            {
                TempData["Message"] = "Payment record not found for this order";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            var allowed = new[] { "Pending", "Completed", "Failed", "Refunded" };
            if (!allowed.Contains(paymentStatus))
            {
                TempData["Message"] = "Invalid payment status";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            payment.PaymentStatus = paymentStatus;
            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();
            TempData["Message"] = $"Payment status updated to {paymentStatus}";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            await _orderRepository.RemoveAsync(order);
            await _orderRepository.SaveChangesAsync();
            TempData["Message"] = "Order deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
