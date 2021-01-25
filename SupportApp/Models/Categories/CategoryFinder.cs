using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SupportApp.Data;
using SupportApp.Models.Tickets;

namespace SupportApp.Models.Categories
{
    public interface ICategoryFinder
    {
        Category Find(int id);
        List<Category> FindAll();
    }
    
    public class CategoryFinder : ICategoryFinder
    {
        private readonly ISupportAppContext _context;

        public CategoryFinder(ISupportAppContext context)
        {
            _context = context;
        }


        public Category Find(int id)
        {
            return _context.FindById<Category>(id);
        }

        public List<Category> FindAll()
        {
            return _context.Queryable<Category>().ToList();
        }
    }
}