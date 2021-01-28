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
        private readonly ITicketsModifier _ticketsModifier;

        public CommentController(ICommentModifier commentModifier, ICommentFinder commentFinder, ITicketsFinder ticketFinder, ITicketsModifier ticketsModifier)
        {
            _commentModifier = commentModifier;
            _commentFinder = commentFinder;
            _ticketFinder = ticketFinder;
            _ticketsModifier = ticketsModifier;
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCommentModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var ticket = _ticketFinder.Find(model.TicketId);
            ticket.AddComment(model.ToDomainObject());
            _ticketsModifier.UpdateTicket(ticket);
            return RedirectToAction("Details","Tickets", new {Id = model.TicketId});

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
            return RedirectToAction("Index","Tickets");
        }
    }
}