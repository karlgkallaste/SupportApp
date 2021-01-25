using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupportApp.Models.Categories;
using SupportApp.Models.Tickets;

namespace SupportApp.ViewModels.Tickets
{
    public class TicketCreateViewModel
    {
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}