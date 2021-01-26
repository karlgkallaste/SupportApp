using System;
using System.ComponentModel.DataAnnotations;
using SupportApp.Models.Categories;

namespace SupportApp.ViewModels.Tickets
{
    public class TicketListViewModel
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Deadline { get; set; }
        
        public Category CategoryName { get; set; }
        
        public string Author { get; set; }
        
    }
}