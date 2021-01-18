using System;
using System.ComponentModel.DataAnnotations;

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
        public DateTime? CompletedAt { get; private set; }
        public bool IsCompleted { get; private set; }
        public Ticket()
        {
            CreatedAt = DateTime.Now;
            Deadline = CreatedAt.AddDays(2);
        }

        public void MarkDone()
        {
            CompletedAt = DateTime.Now;
            IsCompleted = true;
        }

        public void MarkUndone()
        {
            CompletedAt = null;
            IsCompleted = false;
        }
    }
}
