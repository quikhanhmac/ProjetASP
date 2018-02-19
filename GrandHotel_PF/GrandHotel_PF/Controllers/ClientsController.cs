using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_PF.Data;
using GrandHotel_PF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data.SqlClient;
using System.Data;
using GrandHotel_PF.Extensions;

namespace GrandHotel_PF.Controllers
{
    public class ClientsController : Controller
    {
        private readonly GrandHotelDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientsController(GrandHotelDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ListeClient(char id)
        {

            // Requête SQL optimisée: on ramène uniquement les infos nécessaires
            string req = @" select C.Id, C.Civilite, C.Nom, C.Prenom, C.Email, count(R.IdClient) NbReservations, REC.ReservationEnCours
                          from Client C
                          left outer join Reservation R on R.IdClient=C.Id
                          left join vwResaEnCours REC on REC.Id=R.IdClient
                          group by C.Id, C.Civilite, C.Nom, C.Prenom, C.Email, REC.ReservationEnCours
                          having left(C.Nom,1)=@id
                          order by C.Nom";



            var param = new SqlParameter("@id", SqlDbType.Char);
            param.Value = id;

            var clients = new List<ClientVM>();
            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);


                cmd.Parameters.Add(param);
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var c = new ClientVM();
                        c.Client = new Client();
                        c.Client.Id = (int)sdr["Id"];
                        c.Client.Civilite = (string)sdr["Civilite"];
                        c.Client.Nom = (string)sdr["Nom"];
                        c.Client.Prenom = (string)sdr["Prenom"];
                        c.Client.Email = (string)sdr["Email"];
                        c.NbReservations = (int)sdr["NbReservations"];

                        if (sdr["ReservationEnCours"] != DBNull.Value)
                        {
                            c.ReservationEnCours = (int?)sdr["ReservationEnCours"];
                        }
                        else if (sdr["ReservationEnCours"] == DBNull.Value)
                        {
                            c.ReservationEnCours = 0;
                        }

                        if (c.ReservationEnCours != 0)
                        {
                            c.ResEnCours = "OUI";
                        }
                        else
                        {
                            c.ResEnCours = "NON";
                        }


                        clients.Add(c);

                    }
                }
            }
            return View(clients);

        }



        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }



        //[Authorize]
        // GET: Clients/Create //string Email
        public IActionResult Create(string Email)             // Once an account is created the user is redirected here to give his coordinates. 
            // The client cannot give a different email than the one given for his authentication account because that field is already filled.
        {
            return View(); // At the end of this view, by clicking the button "Enregistrer" we redirect the program to the action CreatePost that actually creates the Client
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]  //, string Email
        public async Task<IActionResult> CreatePost([Bind("Id,Civilite,Nom,Prenom,Email,CarteFidelite,Societe")] Client client, string Email)
        {


            if (ModelState.IsValid)
            {
                Email = client.Email;
                _context.Add(client);
                await _context.SaveChangesAsync();


                var reservations = HttpContext.Session.GetObjectFromJson<List<Reservation>>("ResaFinal");

                if (reservations != null) // We test here if there is a reservation running. If yes we redirect to Confirm to finish the reservation. If not we go to the last url.
                {
                    return RedirectToAction("Confirm", "Reservations");  //, new {Email=user.Email }
                }
                else
                {
                    var returnUrl = HttpContext.Session.GetObjectFromJson<string>("url"); // We recover the url saved in the Register process of the authentication
                    return RedirectToLocal(returnUrl);
                }

            }
            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            id = (_context.Client.FirstOrDefault(c => c.Email == user.Email)).Id;

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Civilite,Nom,Prenom,Email,CarteFidelite,Societe")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .SingleOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.SingleOrDefaultAsync(m => m.Id == id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }
    }
}
