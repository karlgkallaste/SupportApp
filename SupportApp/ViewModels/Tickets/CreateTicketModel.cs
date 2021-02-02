using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportApp.Areas.Identity.Data;
using SupportApp.Models.Categories;
using SupportApp.Models.Tickets;

namespace SupportApp.ViewModels.Tickets
{
    public class CreateTicketModel
    {   
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Category { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        
        
        public Ticket ToDomainObject(Guid userId)
        {
            var ticket = new Ticket(Description, Title, userId, CreatedAt, Category);
            CreatedAt = DateTime.Now;

            return ticket;
        }
    }
}