using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrandHotel_PF.Data;
using GrandHotel_PF.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GrandHotel_PF.Controllers
{
    //[Authorize]
    public class ChambresController : Controller
    {
        private readonly GrandHotelDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public ChambresController(GrandHotelDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Chambres
        public async Task<IActionResult> Index(int etat)
        {

            // Requête SQL optimisée: on ramène uniquement les infos nécessaires
            // Le but est de récupérer toutes les informations de toutes les chambres pour savoir si elles sont disponibles ou occupées à la date du jour
            // Elaboration d'une vue pour récupérer dans un premier temps les chambres occupées 
            // Les paramètres id Client(utile pour la gestion interne) et Jour permettent d'indiquer l'état de la chambre 
            string req = @"  select C.Numero, C.Etage, C.Bain, C.Douche, C.WC, C.NbLits, C.NumTel, TC.CodeTarif,T.Prix,ECO.Jour, ECO.IdClient
                             from Chambre C
			                 left outer join vwEtatChambresOccupees ECO on ECO.NumChambre=C.Numero
                             left outer join TarifChambre TC on TC.NumChambre=C.Numero
                             left outer join Tarif T on T.Code=TC.CodeTarif
                             where year(T.DateDebut)=year(getdate())";

            var chambre = new List<ChambreVM>();
            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var c = new ChambreVM();
                        c.Chambre = new Chambre();
                        c.Reservation = new Reservation();

                        c.Chambre.Numero = (short)sdr["Numero"];
                        c.Chambre.Etage = (byte)sdr["Etage"];
                        c.Chambre.Bain = (bool)sdr["Bain"];
                        c.Chambre.Douche = (bool)sdr["Douche"];
                        c.Chambre.Wc = (bool)sdr["WC"];
                        c.Chambre.NbLits = (byte)sdr["NbLits"];
                        c.Chambre.NumTel = (short)sdr["NumTel"];
                        c.CodeTarif = (string)sdr["CodeTarif"];
                        c.Prix = (decimal)sdr["Prix"];

                        if (sdr["Jour"] != DBNull.Value)
                        {
                            c.Jour = (DateTime)sdr["Jour"];
                        }
                        else
                        {
                            c.Jour = DateTime.Today;
                        }

                        if (sdr["IdClient"] != DBNull.Value)
                        {
                            c.Reservation.IdClient = (int)sdr["IdClient"];
                        }

                        // Création d'une colonne: Etat d'occupation de la chambre à la date du jour (Disponible/Occupée)

                        if (c.Reservation.IdClient != 0)
                        {
                            c.EtatChambre = "Chambre Occupée";
                        }
                        else
                        {
                            c.EtatChambre = "Chambre DIsponible";
                        }

                        //etat = c.EtatChambre;

                        chambre.Add(c);

                    }
                }
            }

            return View(chambre);

        }

        // GET: Chambres
        public async Task<IActionResult> ChambreDisponible()
        {
            // Récupération de toutes les chambres disponibles à la date du jour par soustraction 
            // Requête SQL optimisée: on ramène uniquement les infos nécessaires
            string req = @" select C.Numero, C.Etage, C.Bain, C.Douche, C.WC, C.NbLits, C.NumTel, TC.CodeTarif,T.Prix,ECO.Jour, ECO.IdClient
            from Chambre C
			left outer join vwEtatChambresOccupees ECO on ECO.NumChambre=C.Numero
            left outer join TarifChambre TC on TC.NumChambre=C.Numero
            left outer join Tarif T on T.Code=TC.CodeTarif
            where year(T.DateDebut)=year(getdate())
			except
            select C.Numero, C.Etage, C.Bain, C.Douche, C.WC, C.NbLits, C.NumTel, TC.CodeTarif,T.Prix,ECO.Jour, ECO.IdClient
            from Chambre C
			left outer join vwEtatChambresOccupees ECO on ECO.NumChambre=C.Numero
            left outer join TarifChambre TC on TC.NumChambre=C.Numero
            left outer join Tarif T on T.Code=TC.CodeTarif
            where year(T.DateDebut)=year(getdate()) and IdClient!=0";

            var chambre = new List<ChambreVM>();
            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var c = new ChambreVM();
                        c.Chambre = new Chambre();
                        c.Reservation = new Reservation();

                        c.Chambre.Numero = (short)sdr["Numero"];
                        c.Chambre.Etage = (byte)sdr["Etage"];
                        c.Chambre.Bain = (bool)sdr["Bain"];
                        c.Chambre.Douche = (bool)sdr["Douche"];
                        c.Chambre.Wc = (bool)sdr["WC"];
                        c.Chambre.NbLits = (byte)sdr["NbLits"];
                        c.Chambre.NumTel = (short)sdr["NumTel"];
                        c.CodeTarif = (string)sdr["CodeTarif"];
                        c.Prix = (decimal)sdr["Prix"];

                        if (sdr["Jour"] != DBNull.Value)
                        {
                            c.Jour = (DateTime)sdr["Jour"];
                        }
                        else
                        {
                            c.Jour = DateTime.Today;
                        }

                        if (sdr["IdClient"] != DBNull.Value)
                        {
                            c.Reservation.IdClient = (int)sdr["IdClient"];
                        }

                        // Création d'une colonne: Etat d'occupation de la chambre à la date du jour (Disponible/Occupée)

                        if (c.Reservation.IdClient != 0)
                        {
                            c.EtatChambre = "Chambre Occupée";
                        }
                        else
                        {
                            c.EtatChambre = "Chambre DIsponible";
                        }

                        chambre.Add(c);

                    }
                }
            }


            return View(chambre);

        }

        // Récupération de toutes les tables occupées (première étape)
        public async Task<IActionResult> ChambreOccupee()
        {

            // Requête SQL optimisée: on ramène uniquement les infos nécessaires
            string req = @" select C.Numero, C.Etage, C.Bain, C.Douche, C.WC, C.NbLits, C.NumTel, TC.CodeTarif,T.Prix,ECO.Jour, ECO.IdClient
            from Chambre C
			left outer join vwEtatChambresOccupees ECO on ECO.NumChambre=C.Numero
            left outer join TarifChambre TC on TC.NumChambre=C.Numero
            left outer join Tarif T on T.Code=TC.CodeTarif
            where year(T.DateDebut)=year(getdate()) and IdClient!=0";

            var chambre = new List<ChambreVM>();
            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var c = new ChambreVM();
                        c.Chambre = new Chambre();
                        c.Reservation = new Reservation();

                        c.Chambre.Numero = (short)sdr["Numero"];
                        c.Chambre.Etage = (byte)sdr["Etage"];
                        c.Chambre.Bain = (bool)sdr["Bain"];
                        c.Chambre.Douche = (bool)sdr["Douche"];
                        c.Chambre.Wc = (bool)sdr["WC"];
                        c.Chambre.NbLits = (byte)sdr["NbLits"];
                        c.Chambre.NumTel = (short)sdr["NumTel"];
                        c.CodeTarif = (string)sdr["CodeTarif"];
                        c.Prix = (decimal)sdr["Prix"];

                        if (sdr["Jour"] != DBNull.Value)
                        {
                            c.Jour = (DateTime)sdr["Jour"];
                        }
                        else
                        {
                            c.Jour = DateTime.Today;
                        }

                        if (sdr["IdClient"] != DBNull.Value)
                        {
                            c.Reservation.IdClient = (int)sdr["IdClient"];
                        }

                        // Création d'une colonne: Etat d'occupation de la chambre à la date du jour (Disponible/Occupée)

                        if (c.Reservation.IdClient != 0)
                        {
                            c.EtatChambre = "Chambre Occupée";
                        }
                        else
                        {
                            c.EtatChambre = "Chambre DIsponible";
                        }

                        chambre.Add(c);

                    }
                }
            }


            return View(chambre);

        }

        // GET: Chambres/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        // GET: Chambres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chambres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,Etage,Bain,Douche,Wc,NbLits,NumTel")] Chambre chambre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chambre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chambre);
        }

        // GET: Chambres/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre.SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }
            return View(chambre);
        }

        // POST: Chambres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Numero,Etage,Bain,Douche,Wc,NbLits,NumTel")] Chambre chambre)
        {
            if (id != chambre.Numero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chambre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChambreExists(chambre.Numero))
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
            return View(chambre);
        }

        // GET: Chambres/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chambre = await _context.Chambre
                .SingleOrDefaultAsync(m => m.Numero == id);
            if (chambre == null)
            {
                return NotFound();
            }

            return View(chambre);
        }

        // POST: Chambres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var chambre = await _context.Chambre.SingleOrDefaultAsync(m => m.Numero == id);
            _context.Chambre.Remove(chambre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChambreExists(short id)
        {
            return _context.Chambre.Any(e => e.Numero == id);
        }
    }
}
