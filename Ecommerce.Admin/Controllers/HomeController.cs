using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Admin.Models;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Admin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Order> _orderRepository;

    public HomeController(
        ILogger<HomeController> logger,
        IRepository<Product> productRepository,
        IRepository<Category> categoryRepository,
        IRepository<Order> orderRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllAsync();
        var categories = await _categoryRepository.GetAllAsync();
        var orders = await _orderRepository.GetAllAsync();
        var today = DateTime.UtcNow.Date;
        var todayOrders = await _orderRepository.FindAsync(o => o.OrderDate.Date == today);
        var lowStock = await _productRepository.FindAsync(p => p.Stock <= 5);

        var vm = new AdminDashboardViewModel
        {
            TotalProducts = products.Count(),
            TotalCategories = categories.Count(),
            TotalOrders = orders.Count(),
            TodayOrders = todayOrders.Count(),
            LowStockProducts = lowStock.Count(),
            TotalRevenue = orders.Sum(o => o.TotalAmount)
        };

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
