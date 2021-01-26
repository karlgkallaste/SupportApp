using System;
using System.ComponentModel.DataAnnotations;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;

namespace SupportApp.ViewModels.Comments
{
    public class CreateCommentModel
    {   
        [Required]
        public string Content { get; set; }

        public Comment ToDomainObject()
        {
            var comment = new Comment(Content);
            return comment;
        }
    }
}