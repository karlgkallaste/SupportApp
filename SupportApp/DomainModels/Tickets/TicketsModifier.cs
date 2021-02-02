using System;
using System.Linq;
using SupportApp.Data;

namespace SupportApp.Models.Tickets
{
    public interface ITicketsModifier
    {
        Ticket Add(Ticket ticket);
        void RemoveTicket(int id);

        Ticket UpdateTicket(Ticket ticket);

        void RemoveAllByAuthor(Guid author);
    }
    public class TicketsModifier : ITicketsModifier
    {
        private readonly ISupportAppContext _context;

        public TicketsModifier(ISupportAppContext context)
        {
            _context = context;
        }
        public Ticket Add(Ticket ticket)
        {
            var ticketSet = _context.Set<Ticket>();
            ticketSet.Add(ticket);
            _context.SaveChanges();
            return ticket;
        }
        public void RemoveTicket(int id)
        {
            var ticketSet = _context.Set<Ticket>();
            var ticket = _context.FindById<Ticket>(id);
            if (ticket == null)
            {
                return;
            }
            ticketSet.Remove(ticket);
            _context.SaveChanges();
        }

        public Ticket UpdateTicket(Ticket ticket)
        {
            var ticketSet = _context.Set<Ticket>();
            ticketSet.Update(ticket);
            _context.SaveChanges();
            return ticket;
        }

        public void RemoveAllByAuthor(Guid author)
        {
            var ticketSet = _context.Set<Ticket>();
            var tickets = _context.Queryable<Ticket>().Where(t => t.Author == author).ToArray();

            ticketSet.RemoveRange(tickets);
            _context.SaveChanges();
        }
    }
}