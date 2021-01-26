using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupportApp.Models.Categories;
using SupportApp.ViewModels.Categories;

namespace SupportApp.Models.Tickets
{
    public class Ticket
    {
        [Key] 
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime Deadline { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsCompleted { get; private set; }

        public int CategoryId { get; private set; }
        public virtual Category Category { get; private set; }
        
        public Ticket(string description, string title, string author, DateTime createdAt, int categoryId)
        {
            CreatedAt = DateTime.Now;
            Deadline = CreatedAt.AddDays(2);
            Description = description;
            Title = title;
            Author = author;
            CategoryId = categoryId;

        }

        public void MarkDone()
        {
            CompletedAt = DateTime.Now;
            IsCompleted = true;
        }

        public void EditTicket(string description, string title, int categoryId)
        {
            Description = description;
            Title = title;
            CategoryId = categoryId;
        }

        public void MarkUndone()
        {
            CompletedAt = null;
            IsCompleted = false;
        }
    }
}
