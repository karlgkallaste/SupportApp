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
        public DateTime CreatedAt { get; set; }

        public Ticket ToDomainObject()
        {
            var ticket = new Ticket(Description, Title, Author, CreatedAt);
            CreatedAt = DateTime.Now;
            
            return ticket;
        }
    }
}