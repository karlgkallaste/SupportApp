using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Comments;

namespace SupportApp.Controllers
{
    [Route("[controller]/[action]")]
    public class CommentController : Controller
    {
        private readonly ICommentModifier _commentModifier;
        private readonly ICommentFinder _commentFinder;
        private readonly ITicketsFinder _ticketFinder;

        public CommentController(ICommentModifier commentModifier, ICommentFinder commentFinder, ITicketsFinder ticketFinder)
        {
            _commentModifier = commentModifier;
            _commentFinder = commentFinder;
            _ticketFinder = ticketFinder;
        }

        // GET
        //Tested
        [HttpGet("")]
        public IActionResult Index()
        {
            var comments = _commentFinder.FindAll();
            var viewModels = comments.Select(t => new CommentListViewModel
            {
                Id = t.Id,
                Content = t.Content
            }).ToArray();
            return View(viewModels);
        }
        
        
     /*   public IActionResult Create(int id)
        {
            var comments = _ticketFinder.Find(id);
            var viewModels = comments.Select(t => new CreateCommentModel
            {
                Content = t.Content
            }).ToArray();
            return View();
        }
        */
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCommentModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _commentModifier.Add(model.ToDomainObject());
            return RedirectToAction(nameof(Index));

        }
        [HttpGet("{id}")]
        public IActionResult Delete(int id)
        {
            var comment = _commentFinder.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _commentModifier.RemoveComment(id);
            return RedirectToAction(nameof(Index));
        }
    }
}