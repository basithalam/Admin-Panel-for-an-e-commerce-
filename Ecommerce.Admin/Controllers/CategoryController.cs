using Microsoft.AspNetCore.Mvc;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository; // Repository for Category CRUD

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository; // Inject repository
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync(); // Load all categories
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id); // Load single category
            if (category == null) return NotFound();
            return View(category);
        }

        public IActionResult Create()
        {
            return View(new Category()); // Show empty form
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category); // Validate model
            await _categoryRepository.AddAsync(category); // Add entity
            await _categoryRepository.SaveChangesAsync(); // Persist changes
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id); // Load entity
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId) return BadRequest(); // Ensure id matches
            if (!ModelState.IsValid) return View(category); // Validate
            await _categoryRepository.UpdateAsync(category); // Update entity
            await _categoryRepository.SaveChangesAsync(); // Persist
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id); // Load entity
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id); // Load entity
            if (category == null) return NotFound();
            await _categoryRepository.RemoveAsync(category); // Remove entity
            await _categoryRepository.SaveChangesAsync(); // Persist
            return RedirectToAction(nameof(Index));
        }
    }
}
