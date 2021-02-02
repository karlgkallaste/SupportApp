using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportApp.Models.Categories;
using SupportApp.ViewModels.Categories;

namespace SupportApp.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Admin")]
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
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCategoryModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _categoryModifier.Add(model.ToDomainObject());
            return RedirectToAction(nameof(Index));

        }
        
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
        
        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryModifier.RemoveCategory(id);
            return RedirectToAction(nameof(Index));
        }
    }
}