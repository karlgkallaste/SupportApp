using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SupportApp.Data;
using SupportApp.Models.Tickets;
using SupportApp.Models.Comments;

namespace SupportApp.Models.Comments
{
    public interface ICommentFinder
    {
        Comment Find(int id);
        List<Comment> FindAll();
        IReadOnlyCollection<Comment> FindTicketComments(int id);
    }
    
    public class CommentFinder : ICommentFinder
    {
        private readonly ISupportAppContext _context;

        public CommentFinder(ISupportAppContext context)
        {
            _context = context;
        }


        public Comment Find(int id)
        {
            return _context.FindById<Comment>(id);
        }

        public List<Comment> FindAll()
        {
            return _context.Queryable<Comment>().ToList();
        }
        public IReadOnlyCollection<Comment> FindTicketComments(int id)
        {
            return _context.Queryable<Comment>().Where(t => t.TicketId == id).ToArray();
        }
    }
}