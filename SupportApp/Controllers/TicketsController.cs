using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportApp.Areas.Identity.Data;
using SupportApp.Extensions;
using SupportApp.Infrastructure;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Images;
using SupportApp.Models.Tickets;
using SupportApp.ViewModels.Comments;
using SupportApp.ViewModels.Tickets;

namespace SupportApp.Controllers
{
    [Route("[controller]/[action]")]
    public class TicketsController : Controller
    {
        private readonly ITicketsFinder _ticketsFinder;
        private readonly ITicketsModifier _ticketsModifier;
        private readonly ICategoryFinder _categoryFinder;
        private readonly IMemoryStreamProvider _memoryStreamProvider;


        public TicketsController(ITicketsFinder ticketsFinder, ITicketsModifier ticketsModifier,
            ICategoryFinder categoryFinder, IMemoryStreamProvider memoryStreamProvider)
        {
            _ticketsFinder = ticketsFinder;
            _ticketsModifier = ticketsModifier;
            _categoryFinder = categoryFinder;
            _memoryStreamProvider = memoryStreamProvider;
        }

        // GET: Tickets
        [Authorize]
        public IActionResult Index(string searchString)
        {
            var incompleteTickets = User.IsInRole("Admin")
                ? _ticketsFinder.FindAllWithStatus(false, searchString)
                : _ticketsFinder.FindAllByAuthor(User.GetUserId(), false, searchString);

            var userViewModels = incompleteTickets.Select(t => new TicketListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category.Name
            }).ToArray();
            return View(userViewModels);
        }

        // GET: Completed Tickets
        public IActionResult Completed(string searchString)
        {
            var completeTickets = User.IsInRole("Admin")
                ? _ticketsFinder.FindAllWithStatus(true, searchString)
                : _ticketsFinder.FindAllByAuthor(User.GetUserId(), true, searchString);

            var userViewModels = completeTickets.Select(t => new TicketListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category.Name
            }).ToArray();
            return View(userViewModels);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult OverdueTickets()
        {
            var incompleteTickets = _ticketsFinder.FindAllByOverDueTickets(false);
            var viewModels = incompleteTickets.Select(t => new OverDueTicketListViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Category = t.Category.Name,
                Deadline = t.Deadline,
                Hourspast = (int) DateTime.Now.Subtract(t.Deadline).TotalHours
            }).ToArray();
            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(int id, List<IFormFile> files)
        {
            if (files.Any(f => f.Length > 2097152))
            {
                ModelState.AddModelError("Images", "Maksimaalne faili suurus on 2 MB.");
                return Details(id);
            }

            var ticket = _ticketsFinder.Find(id);
            if (ticket.AuthorId != User.GetUserId() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            foreach (var formFile in files)
            {
                using (var memoryStream = _memoryStreamProvider.Provide())
                {
                    await formFile.CopyToAsync(memoryStream);

                    var file = new Image(memoryStream.ToArray());
                    ticket.AddImage(file);
                }
            }

            _ticketsModifier.UpdateTicket(ticket);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: Tickets/Details/5
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.AuthorId != User.GetUserId() && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var viewModel = new TicketDetailsViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Deadline = ticket.Deadline,
                Author = ticket.Author,
                CreatedAt = ticket.CreatedAt,
                CompletedAt = ticket.CompletedAt,
                IsCompleted = ticket.IsCompleted,
                Category = ticket.Category.Name,
                Images = ticket.GetImages().Select(i => new ImageViewModel()
                {
                    DataBase64 = Convert.ToBase64String(i.Data)
                }).ToList(),
                Comments = ticket.GetComments().Select(c => new CommentListViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                }).ToList()
            };

            return View(nameof(Details), viewModel);
        }

        // GET: Tickets/Create
        [Authorize]
        public IActionResult Create()
        {
            // Get Post entity from database
            // Populate list of possible categories for post & add to model
            var categories = _categoryFinder.FindAll();
            var categoryOptions = categories.Select(c => new SelectListItem
            {
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateTicketModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _ticketsModifier.Add(model.ToDomainObject(User.GetUserId(),User.Identity.Name));
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: Tickets/Edit/5
        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.AuthorId != User.GetUserId() && User.IsInRole("Admin").Equals(false))
            {
                return Forbid();
            }

            var categories = _categoryFinder.FindAll();
            var categoryOptions = categories.Select(c => new SelectListItem
            {
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
        [Authorize]
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

                if (ticket.AuthorId != User.GetUserId() && User.IsInRole("Admin").Equals(false))
                {
                    return Forbid();
                }

                ticket.EditTicket(model.Description, model.Title, model.CategoryId);

                // if (model.IsCompleted && !ticket.IsCompleted)
                // {
                //     ticket.MarkDone();
                // }
                // else if (!model.IsCompleted && ticket.IsCompleted)
                // {
                //     ticket.MarkUndone();
                // }

                _ticketsModifier.UpdateTicket(ticket);
                return RedirectToAction(nameof(Index));
            }

            var categories = _categoryFinder.FindAll();
            var categoryOptions = categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToArray();
            model.Categories = categoryOptions;
            return View(model);
        }

        // GET: Tickets/Delete/5
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Delete(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ticket.AuthorId != User.GetUserId() && User.IsInRole("Admin").Equals(false))
            {
                return Forbid();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost("{id}"), ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _ticketsModifier.RemoveTicket(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Tickets/RemoveByAuthor/Name
        [ActionName("RemoveByAuthor")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveByAuthor()
        {
            _ticketsModifier.RemoveAllByAuthor(User.GetUserId());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("{id}")]
        [Authorize]
        public IActionResult MarkDone(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            ticket.MarkDone();
            _ticketsModifier.UpdateTicket(ticket);
            return RedirectToAction(nameof(Index));
        }
    }
}