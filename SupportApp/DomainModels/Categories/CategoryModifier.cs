using SupportApp.Data;

namespace SupportApp.Models.Categories
{
    public interface ICategoryModifier
    {
        Category Add(Category category);
        void RemoveCategory(int id);

    }
    public class CategoryModifier : ICategoryModifier
    {
        private readonly ISupportAppContext _context;

        public CategoryModifier(ISupportAppContext context)
        {
            _context = context;
        }
        public Category Add(Category category)
        {
            var categorySet = _context.Set<Category>();
            categorySet.Add(category);
            _context.SaveChanges();
            return category;
        }
        public void RemoveCategory(int id)
        {
            var categorySet = _context.Set<Category>();
            var category = _context.FindById<Category>(id);
            if (category == null)
            {
                return;
            }
            categorySet.Remove(category);
            _context.SaveChanges();
        }
    }
}