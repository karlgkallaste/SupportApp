using System;
using System.Collections;
using System.Collections.Generic;
using SupportApp.Models.Comments;
using SupportApp.ViewModels.Comments;

namespace SupportApp.ViewModels.Tickets
{
    public class TicketDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime Deadline { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }

        public virtual List<CommentListViewModel> Comments { get; set; } = new List<CommentListViewModel>();
    }
}