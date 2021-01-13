using System.Collections.Generic;
using System.Linq;
using SupportApp.Data;

namespace SupportApp.Models.Tickets
{
    public interface ITicketsFinder
    {
        Ticket Find(int id);
        IReadOnlyCollection<Ticket> FindAllWithStatus(int status);
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

        public IReadOnlyCollection<Ticket> FindAllWithStatus(int status)
        {
            return _context.Queryable<Ticket>().Where(t => t.Status == status).ToArray();
        }
    }
}