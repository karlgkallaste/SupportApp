using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportApp.Data;
using SupportApp.Models;

namespace SupportApp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ISupportAppRepository<Ticket> _ticketRepository;

        public TicketsController(ISupportAppRepository<Ticket> ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            var incompleteTickets = _ticketRepository.Find(ticket => ticket.Status == 0)
                .OrderBy(ticket => ticket.CreatedAt)
                .ToArray();
            return View(incompleteTickets);
        }

        // GET: Completed Tickets
        public IActionResult Completed()
        {
            return View(_ticketRepository.Find(m => m.Status == 1).ToArray());
        }

        // GET: Tickets/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketRepository.GetById(id.Value);
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
                _ticketRepository.Add(ticket);
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketRepository.GetById(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
                var ticket = _ticketRepository.GetById(id);
                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.Author = model.Author;
                ticket.CompletedAt = model.CompletedAt;
                ticket.Status = model.Status;
                
                _ticketRepository.Update(ticket);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = _ticketRepository.GetById(id.Value);
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
            var ticket = _ticketRepository.GetById(id);
            if (ticket == null)
            {
                return RedirectToAction(nameof(Index));
            }
            _ticketRepository.Remove(ticket);
            return RedirectToAction(nameof(Index));
        }
    }
}