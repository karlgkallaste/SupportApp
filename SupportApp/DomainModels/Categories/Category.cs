using System.Linq;
using Microsoft.EntityFrameworkCore;
using SupportApp.Data;

namespace SupportApp.Models.Categories
{
    public class Category
    {
        public Category(string name)
        {
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        
    }
}