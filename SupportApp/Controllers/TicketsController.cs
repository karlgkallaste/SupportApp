using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SupportApp.Models.Tickets;

namespace SupportApp.Controllers
{
    [Route("[controller]/[action]")]
    public class TicketsController : Controller
    {
        private readonly ITicketsFinder _ticketsFinder;
        private readonly ITicketsModifier _ticketsModifier;

        public TicketsController(ITicketsFinder ticketsFinder, ITicketsModifier ticketsModifier)
        {
            _ticketsFinder = ticketsFinder;
            _ticketsModifier = ticketsModifier;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            // var incompleteTickets = _ticketRepository.Find(ticket => ticket.Status == 0)
            //     .OrderBy(ticket => ticket.CreatedAt)
            //     .ToArray();
            var incompleteTickets = _ticketsFinder.FindAllWithStatus(0);
            return View(incompleteTickets);
        }

        // GET: Completed Tickets
        public IActionResult Completed()
        {
            var completeTickets = _ticketsFinder.FindAllWithStatus(1);
            return View(completeTickets);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,Description,Author,CreatedAt,CompletedAt,Status")]
            Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _ticketsModifier.Add(ticket);
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Author,CreatedAt,CompletedAt,Status")]
            Ticket model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var ticket = _ticketsFinder.Find(id);
                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.Author = model.Author;
                ticket.CompletedAt = model.CompletedAt;
                ticket.Status = model.Status;
                
                _ticketsModifier.EditTicket(ticket);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = _ticketsFinder.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _ticketsModifier.RemoveTicket(id);
            return RedirectToAction(nameof(Index));
        }
    }
}