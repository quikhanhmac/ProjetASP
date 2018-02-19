using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_PF.Data;
using GrandHotel_PF.Models;

namespace GrandHotel_PF.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly GrandHotelDbContext _context;

        public ReservationsController(GrandHotelDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var grandHotelDbContext = _context.Reservation.Include(r => r.IdClientNavigation).Include(r => r.JourNavigation).Include(r => r.NumChambreNavigation);
            return View(await grandHotelDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.IdClientNavigation)
                .Include(r => r.JourNavigation)
                .Include(r => r.NumChambreNavigation)
                .SingleOrDefaultAsync(m => m.NumChambre == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["NbPersonne"] = new SelectList(_context.Set<Reservation>(), "NbPersonne", "NbPersonne");
            ViewData["Jour"] = new SelectList(_context.Set<Calendrier>(), "Jour", "Jour");
            ViewData["HeureArrivee"] = new SelectList(_context.Set<Reservation>(), "HeureArrivee", "HeureArrivee");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Jour,NbPersonnes,HeureArrivee,Travail")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NbPersonne"] = new SelectList(_context.Set<Reservation>(), "NbPersonne", "NbPersonne", reservation.NbPersonnes);
            ViewData["Jour"] = new SelectList(_context.Set<Calendrier>(), "Jour", "Jour", reservation.Jour);
            ViewData["HeureArrivee"] = new SelectList(_context.Set<Reservation>(), "HeureArrivee", "HeureArrivee", reservation.HeureArrivee);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.SingleOrDefaultAsync(m => m.NumChambre == id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Set<Client>(), "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Set<Calendrier>(), "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Set<Chambre>(), "Numero", "Numero", reservation.NumChambre);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("NumChambre,Jour,IdClient,NbPersonnes,HeureArrivee,Travail")] Reservation reservation)
        {
            if (id != reservation.NumChambre)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.NumChambre))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Set<Client>(), "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Set<Calendrier>(), "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Set<Chambre>(), "Numero", "Numero", reservation.NumChambre);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.IdClientNavigation)
                .Include(r => r.JourNavigation)
                .Include(r => r.NumChambreNavigation)
                .SingleOrDefaultAsync(m => m.NumChambre == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var reservation = await _context.Reservation.SingleOrDefaultAsync(m => m.NumChambre == id);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(short id)
        {
            return _context.Reservation.Any(e => e.NumChambre == id);
        }
    }
}
