using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportApp.Models.Categories;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Tickets;

namespace SupportApp.Controllers
{
    [Route("[controller]/[action]")]
    public class TicketsController : Controller
    {
        private readonly ITicketsFinder _ticketsFinder;
        private readonly ITicketsModifier _ticketsModifier;
        private readonly ICategoryFinder _categoryFinder;

        public TicketsController(ITicketsFinder ticketsFinder, ITicketsModifier ticketsModifier, ICategoryFinder categoryFinder)
        {
            _ticketsFinder = ticketsFinder;
            _ticketsModifier = ticketsModifier;
            _categoryFinder = categoryFinder;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            var incompleteTickets = _ticketsFinder.FindAllWithStatus(false);
            var viewModels = incompleteTickets.Select(t => new TicketListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category.Name
            }).ToArray();
            return View(viewModels);
        }

        // GET: Completed Tickets
        public IActionResult Completed()
        {   
            var completeTickets = _ticketsFinder.FindAllWithStatus(true);
            var viewModels = completeTickets.Select(t => new TicketListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category.Name
            }).ToArray();
            return View(viewModels);
        }

        // GET: Tickets/Details/5
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var viewModel = new TicketDetailsViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Author = ticket.Author,
                Deadline = ticket.Deadline,
                CreatedAt = ticket.CreatedAt,
                CompletedAt = ticket.CompletedAt,
                IsCompleted = ticket.IsCompleted,
                Category = ticket.Category.Name
            };

            return View(viewModel);
        }

        // GET: Tickets/Create
        
        public IActionResult Create()
        {
            // Get Post entity from database
            // Populate list of possible categories for post & add to model
            var categories = _categoryFinder.FindAll();
            var categoryOptions = categories.Select(c=> new SelectListItem {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            // Add post & categories to ViewModel
            // Populate the view's model
            return View(new CreateTicketModel
            {
                Categories = categoryOptions
            });
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateTicketModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _ticketsModifier.Add(model.ToDomainObject());
            return RedirectToAction(nameof(Index));

        }

        // GET: Tickets/Edit/5
        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var categories = _categoryFinder.FindAll();
            var categoryOptions = categories.Select(c=> new SelectListItem {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToArray();
            var viewModel = new TicketEditViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                IsCompleted = ticket.IsCompleted,
                CategoryId = ticket.CategoryId,
                Categories = categoryOptions
            };

            return View(viewModel);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TicketEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ticket = _ticketsFinder.Find(model.Id);
                if (ticket == null)
                {
                    return NotFound();
                }
                
                ticket.EditTicket(model.Description, model.Title, model.CategoryId);
                
                if (model.IsCompleted && !ticket.IsCompleted)
                {
                    ticket.MarkDone();
                }
                else if(!model.IsCompleted && ticket.IsCompleted)
                {
                    ticket.MarkUndone();
                }
                
                _ticketsModifier.UpdateTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            var categories = _categoryFinder.FindAll();
            var categoryOptions = categories.Select(c=> new SelectListItem {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToArray();
            model.Categories = categoryOptions;
            return View(model);
        }

        // GET: Tickets/Delete/5
        [HttpGet("{id}")]
        public IActionResult Delete(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _ticketsModifier.RemoveTicket(id);
            return RedirectToAction(nameof(Index));
        }
        // POST: Tickets/RemoveByAuthor/Name
        [ActionName("RemoveByAuthor")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveByAuthor(string author)
        {
            _ticketsFinder.FindAllByAuthor(author);
            _ticketsModifier.RemoveAllByAuthor(author);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost("{id}")]
        public IActionResult MarkDone(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            ticket.MarkDone();
            _ticketsModifier.UpdateTicket(ticket);
            return RedirectToAction(nameof(Index));
        }
    }
}