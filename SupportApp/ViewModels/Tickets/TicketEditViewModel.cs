using System;

namespace SupportApp.ViewModels.Tickets
{
    public class TicketEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CompletedAt { get; set; }
        public int Status { get; set; }
            
    }
}