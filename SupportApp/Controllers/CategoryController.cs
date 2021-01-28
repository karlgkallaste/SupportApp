using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportApp.Models.Categories;
using SupportApp.ViewModels.Categories;

namespace SupportApp.Controllers
{
    [Route("[controller]/[action]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryModifier _categoryModifier;
        private readonly ICategoryFinder _categoryFinder;

        public CategoryController(ICategoryModifier categoryModifier, ICategoryFinder categoryFinder)
        {
            _categoryModifier = categoryModifier;
            _categoryFinder = categoryFinder;
        }

        // GET
        //Tested
        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public IActionResult Index()
        {
            var categories = _categoryFinder.FindAll();
            var viewModels = categories.Select(t => new CategoryListViewModel
            {
                Id = t.Id,
                Name = t.Name
            }).ToArray();
            return View(viewModels);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCategoryModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _categoryModifier.Add(model.ToDomainObject());
            return RedirectToAction(nameof(Index));

        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _categoryFinder.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryModifier.RemoveCategory(id);
            return RedirectToAction(nameof(Index));
        }
    }
}