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
        public IActionResult Index()
        {
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite");
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour");
            //ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero");
            //var grandHotelDbContext = _context.Reservation.Include(r => r.IdClientNavigation).Include(r => r.JourNavigation).Include(r => r.NumChambreNavigation);
            //return View(await grandHotelDbContext.ToListAsync());
            return View();
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
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite");
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour");
            ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero");
            return View();
        }

        // POST: Reservations/ValiderDisponibilite
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValiderDisponibilite(int NbNuits, DateTime Jour,  byte NbPersonnes)
        {
            //double temp = Convert.ToDouble(Id);
            return View(DAL.GetChambre(NbNuits, Jour,  NbPersonnes));
        }



        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumChambre,Jour,IdClient,NbPersonnes,HeureArrivee,Travail")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero", reservation.NumChambre);
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
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero", reservation.NumChambre);
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
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", reservation.IdClient);
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour", reservation.Jour);
            ViewData["NumChambre"] = new SelectList(_context.Chambre, "Numero", "Numero", reservation.NumChambre);
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
