using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            await BindCategoriesAsync();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Product creation failed due to validation errors: {Errors}", string.Join(", ", errors));
                await BindCategoriesAsync();
                return View(product);
            }
            try
            {
                await _productService.CreateProductAsync(product);
                TempData["Success"] = "Product saved successfully";
                return RedirectToAction(nameof(Create));
            }
            catch (ArgumentException ex) when (ex.ParamName == nameof(Product.CategoryId))
            {
                ModelState.AddModelError(nameof(Product.CategoryId), ex.Message);
                await BindCategoriesAsync();
                return View(product);
            }
            catch (ArgumentException ex) when (ex.ParamName == nameof(Product.Price))
            {
                ModelState.AddModelError(nameof(Product.Price), ex.Message);
                await BindCategoriesAsync();
                return View(product);
            }
            catch (ArgumentException ex) when (ex.ParamName == nameof(Product.Stock))
            {
                ModelState.AddModelError(nameof(Product.Stock), ex.Message);
                await BindCategoriesAsync();
                return View(product);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to save product. Please try again.";
                ModelState.AddModelError(string.Empty, "Unable to save product. Please try again.");
                await BindCategoriesAsync();
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            await BindCategoriesAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest();
            if (!ModelState.IsValid)
            {
                await BindCategoriesAsync();
                return View(product);
            }
            try
            {
                await _productService.UpdateProductAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError(string.Empty, $"Unable to update product: {ex.Message}");
                await BindCategoriesAsync();
                return View(product);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task BindCategoriesAsync()
        {
            var categories = await _productService.GetAllCategoriesAsync();
            ViewBag.CategoryId = new SelectList(categories, nameof(Category.CategoryId), nameof(Category.Name));
        }
    }
}
