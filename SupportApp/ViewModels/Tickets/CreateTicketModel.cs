using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [MaxLength(30)]
        public string Author { get; set; }

        public Ticket ToDomainObject()
        {
            var ticket = new Ticket
            {
                Author = Author,
                Title = Title,
                Description = Description
            };
            return ticket;
        }
    }
}