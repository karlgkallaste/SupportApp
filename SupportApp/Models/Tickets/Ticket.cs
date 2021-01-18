using System;
using System.ComponentModel.DataAnnotations;

namespace SupportApp.Models.Tickets
{
    public class Ticket
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime Deadline { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsCompleted { get; private set; }
        public Ticket(string description, string title, string author, DateTime createdAt)
        {
            CreatedAt = DateTime.Now;
            Deadline = CreatedAt.AddDays(2);
            Description = description;
            Title = title;
            Author = author;

        }

        public void MarkDone()
        {
            CompletedAt = DateTime.Now;
            IsCompleted = true;
        }

        public void EditTicket(string description, string title)
        {
            Description = description;
            Title = title;
        }

        public void MarkUndone()
        {
            CompletedAt = null;
            IsCompleted = false;
        }
    }
}
