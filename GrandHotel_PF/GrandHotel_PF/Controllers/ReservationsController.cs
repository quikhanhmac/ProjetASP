using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_PF.Data;
using GrandHotel_PF.Models;
using GrandHotel_PF.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GrandHotel_PF.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly GrandHotelDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(GrandHotelDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public IActionResult Index()
        {
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite");
            ViewData["Jour"] = new SelectList(_context.Calendrier, "Jour", "Jour");
            return View(); // In this View ("Index") shows the form that has to be filled by the client to search if there are rooms available for the dates he wants
        }

        //[Authorize]
        public IActionResult Details(Reservation reservation)
        {
            _context.SaveChangesAsync();
            var reservations = HttpContext.Session.GetObjectFromJson<List<Reservation>>("ResaFinal");
            // We recover form the memory the reservation with all the information (except for the Id of the client which is given after the creation of his account) in order to show it here with all its details
            ViewBag.duree = reservations.Count;
            return View(reservation);
        }

        [Authorize] // Before confirming the reservation and writing the information in the database we ask for authentication. 
                    // We put this here because a) it makes more sense to ask for authentication right before editing the database and 
                    // b) beacause it is easier to redirect to this action from the controller Client since this action does not take parameters
        public async Task<IActionResult> Confirm()
        {

            var user = await _userManager.GetUserAsync(User); // This is the "active" user

            // Before confirming the reservation
            var reservations = HttpContext.Session.GetObjectFromJson<List<Reservation>>("ResaFinal"); // We recover here the reservation which has all the information stored ecxept for the Id client
            for (int res = 0; res < reservations.Count; res++)
            {
                if (ModelState.IsValid)
                { // Here we add the id of the client to all the reservations (one for each day) that he  created
                    reservations[res].IdClient = (_context.Client.FirstOrDefault(c => c.Email == user.Email)).Id; 
                    _context.Add(reservations[res]);
                }
            }
            await _context.SaveChangesAsync(); // Finally the reservation is saved in the database


            return View();
        }



        // POST: Reservations/ValiderDisponibilite
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValiderDisponibilite(int NbNuits, DateTime Jour, byte NbPersonnes, byte HeureArrivee, bool Travail)
        {

            // Here we create the list of reservations with one reservation for each day (attention the day of departure is not counted because the room should be free by night)
            List<Reservation> reservations = new List<Reservation>();
            for (int jours = 0; jours < NbNuits; jours++)
            {
                Reservation reservation = new Reservation();
                reservation.Jour = Jour.AddDays(jours);
                reservation.IdClient = 0; // For the moment this information is unknown
                reservation.NbPersonnes = NbPersonnes;
                reservation.HeureArrivee = HeureArrivee;
                reservation.Travail = Travail;

                reservations.Add(reservation);
            }
            HttpContext.Session.SetObjectAsJson("Resa", reservations);
            // We save this information in the memory to recover it later and add the rest of the fields that are missing (room number and id of client)

            return View(DAL.GetChambre(NbNuits, Jour, NbPersonnes)); // Here we return to the View the list of available rooms so that he can choose one
        }



        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]

        public IActionResult Create(short Numero)
        {
            var reservations = HttpContext.Session.GetObjectFromJson<List<Reservation>>("Resa");

            if (reservations != null)
            {
                for (int res = 0; res < reservations.Count; res++)
                {
                    // Now that he chose the room we add the info to the reservations
                    reservations[res].NumChambre = Numero;

                }
                //_context.SaveChangesAsync();

                HttpContext.Session.SetObjectAsJson("ResaFinal", reservations);


                return RedirectToAction(nameof(Details), reservations[0]);
            }
            else
            {
                return View("Index");
            }


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
