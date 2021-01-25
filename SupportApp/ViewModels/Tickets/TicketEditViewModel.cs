using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupportApp.ViewModels.Tickets
{
    public class TicketEditViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int CategoryId { get; set; }
        public SelectListItem[] Categories { get; set; }
    }
}