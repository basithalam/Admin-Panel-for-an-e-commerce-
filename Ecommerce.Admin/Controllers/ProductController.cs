using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository; // Repository for products
        private readonly ICategoryRepository _categoryRepository; // Repository for categories

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository; // Inject product repo
            _categoryRepository = categoryRepository; // Inject category repo
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllWithCategoryAsync(); // Load products with category
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdWithCategoryAsync(id); // Load single product
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            await BindCategoriesAsync(); // Prepare category dropdown
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                await BindCategoriesAsync(); // Rebind categories on validation fail
                return View(product);
            }
            await _productRepository.AddAsync(product); // Add entity
            await _productRepository.SaveChangesAsync(); // Persist
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id); // Load product
            if (product == null) return NotFound();
            await BindCategoriesAsync(); // Bind categories
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest(); // Ensure id matches
            if (!ModelState.IsValid)
            {
                await BindCategoriesAsync(); // Rebind dropdowns
                return View(product);
            }
            await _productRepository.UpdateAsync(product); // Update entity
            await _productRepository.SaveChangesAsync(); // Persist
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdWithCategoryAsync(id); // Load with category
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id); // Load entity
            if (product == null) return NotFound();
            await _productRepository.RemoveAsync(product); // Remove entity
            await _productRepository.SaveChangesAsync(); // Persist
            return RedirectToAction(nameof(Index));
        }

        private async Task BindCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync(); // Load categories
            ViewBag.CategoryId = new SelectList(categories, nameof(Category.CategoryId), nameof(Category.Name)); // Build dropdown
        }
    }
}
