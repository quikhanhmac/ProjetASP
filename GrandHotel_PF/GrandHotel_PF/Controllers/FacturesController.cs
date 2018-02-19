using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GrandHotel_PF.Data;
using GrandHotel_PF.Models;

namespace GrandHotel_PF.Controllers


{

    public class FacturesController : Controller
    {
        private readonly GrandHotelDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public FacturesController(GrandHotelDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize]
        // Récupération de la liste des factures pour un client donné (rechercher avec son email)
        public async Task<IActionResult> Index(int annee)
        {

            var user = await _userManager.GetUserAsync(User);

            var facture = _context.Facture.OrderByDescending(d => d.DateFacture).
                Include(f => f.CodeModePaiementNavigation).Include(f => f.IdClientNavigation).
                Where(c => c.IdClientNavigation.Email == user.Email && c.DateFacture.Year == annee);
            return View(await facture.ToListAsync());

        }

        // GET: Factures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Requête SQL optimisée: on ramène uniquement les infos nécessaires
            // Récupération du détail de la facture sélectionnée
            string req = @"select IdFacture, NumLigne, Quantite, MontantHT, TauxTVA, TauxReduction,Quantite*MontantHT*(1+TauxTVA)*(1-TauxReduction) as MontantTotal
                        from LigneFacture
                        where IdFacture=@id";

            var lignefactures = new List<LigneFacture>();

            using (var conn = (SqlConnection)_context.Database.GetDbConnection())
            {
                var cmd = new SqlCommand(req, conn);
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@id", Value = id });

                await conn.OpenAsync();

                using (var sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        var c = new LigneFacture();

                        c.IdFacture = (int)sdr["IdFacture"];
                        c.NumLigne = (int)sdr["NumLigne"];
                        c.Quantite = (short)sdr["Quantite"];
                        c.MontantHt = (decimal)sdr["MontantHt"];
                        c.TauxTva = (decimal)sdr["TauxTVA"];
                        c.TauxReduction = (decimal)sdr["TauxReduction"];
                        c.MontantTotal = (decimal)sdr["MontantTotal"];


                        lignefactures.Add(c);

                    }
                }
            }

            return View(lignefactures);

        }

        // GET: Factures/Create
        public IActionResult Create()
        {
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code");
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite");
            return View();
        }

        // POST: Factures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdClient,DateFacture,DatePaiement,CodeModePaiement")] Facture facture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
            return View(facture);
        }

        // GET: Factures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Facture.SingleOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
            return View(facture);
        }

        // POST: Factures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdClient,DateFacture,DatePaiement,CodeModePaiement")] Facture facture)
        {
            if (id != facture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactureExists(facture.Id))
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
            ViewData["CodeModePaiement"] = new SelectList(_context.ModePaiement, "Code", "Code", facture.CodeModePaiement);
            ViewData["IdClient"] = new SelectList(_context.Client, "Id", "Civilite", facture.IdClient);
            return View(facture);
        }

        // GET: Factures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Facture
                .Include(f => f.CodeModePaiementNavigation)
                .Include(f => f.IdClientNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }

            return View(facture);
        }

        // POST: Factures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facture = await _context.Facture.SingleOrDefaultAsync(m => m.Id == id);
            _context.Facture.Remove(facture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FactureExists(int id)
        {
            return _context.Facture.Any(e => e.Id == id);
        }
    }
}
