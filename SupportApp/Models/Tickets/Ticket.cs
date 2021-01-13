using System;

namespace SupportApp.Models.Tickets
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CompletedAt { get; set; }
        public int Status { get; set; }
        public Ticket()
        {
            CreatedAt = DateTime.Now;
            Deadline = CreatedAt.AddDays(2);
        }
    }
}
