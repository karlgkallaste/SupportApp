using System;
using System.ComponentModel.DataAnnotations;
using SupportApp.Models.Categories;

namespace SupportApp.ViewModels.Categories
{
    public class CreateCategoryModel
    {   
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public Category ToDomainObject()
        {
            var category = new Category(Name);
            return category;
        }
    }
}