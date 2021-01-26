using SupportApp.Data;
using SupportApp.Models.Comments;

namespace SupportApp.Models.Comments
{
    public interface ICommentModifier
    {
        Comment Add(Comment comment);
        void RemoveComment(int id);

    }
    public class CommentModifier : ICommentModifier
    {
        private readonly ISupportAppContext _context;

        public CommentModifier(ISupportAppContext context)
        {
            _context = context;
        }
        public Comment Add(Comment comment)
        {
            var commentSet = _context.Set<Comment>();
            commentSet.Add(comment);
            _context.SaveChanges();
            return comment;
        }
        public void RemoveComment(int id)
        {
            var commentSet = _context.Set<Comment>();
            var comment = _context.FindById<Comment>(id);
            if (comment == null)
            {
                return;
            }
            commentSet.Remove(comment);
            _context.SaveChanges();
        }
    }
}