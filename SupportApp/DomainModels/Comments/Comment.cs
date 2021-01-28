using System.Linq;
using Microsoft.EntityFrameworkCore;
using SupportApp.Data;
using SupportApp.Models.Tickets;

namespace SupportApp.Models.Comments
{
    public class Comment
    {
        public Comment(string content)
        {
            Content = content;
        }

        public int Id { get; private set; }
        public string Content { get; private set; }
        
        public int TicketId { get; private set; }
        
        public virtual Ticket Ticket { get; private set; }
        
    }
}