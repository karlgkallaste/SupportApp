using System.Linq;
using Microsoft.EntityFrameworkCore;
using SupportApp.Data;

namespace SupportApp.Models.Comments
{
    public class Comment
    {
        public Comment(string content)
        {
            Content = content;
        }

        public int Id { get; private set; }
        public string Content { get; internal set; }
        
        public int TicketId { get; set; }
        
    }
}