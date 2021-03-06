using System;
using System.Collections.Generic;
using System.Linq;
using SupportApp.Data;

namespace SupportApp.Models.Tickets
{
    public interface ITicketsFinder
    {
        Ticket Find(int id);
        IReadOnlyCollection<Ticket> FindAllWithStatus(bool completed, string searchString);
        IReadOnlyCollection<Ticket> FindAllByAuthor(Guid author, bool completed, string searchString);
        IReadOnlyCollection<Ticket> FindAllByOverDueTickets(bool completed);

    }
    
    public class TicketsFinder : ITicketsFinder
    {
        private readonly ISupportAppContext _context;

        public TicketsFinder(ISupportAppContext context)
        {
            _context = context;
        }

        public Ticket Find(int id)
        {
            return _context.FindById<Ticket>(id);
        }

        public IReadOnlyCollection<Ticket> FindAllWithStatus(bool completed, string searchString)
        {
            return _context.Queryable<Ticket>().Where(t => t.IsCompleted == completed).OrderBy(t=>t.CreatedAt).ToArray();
        }

        public IReadOnlyCollection<Ticket> FindAllByAuthor(Guid author, bool completed, string searchString)
        {
            return _context.Queryable<Ticket>().Where(t => t.AuthorId == author && t.IsCompleted == completed).ToArray();
        }
        public IReadOnlyCollection<Ticket> FindAllByOverDueTickets(bool completed)
        {
            return _context.Queryable<Ticket>().Where(t=>t.IsCompleted == false && t.Deadline <= DateTime.Now).ToArray();
        }
    }
}