using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Route("[Controller]/[Action]")]
    public class TachesController : Controller
    {
        private readonly TodoListContext _context;

        public TachesController(TodoListContext context)
        {
            _context = context;
        }

        // GET: Taches
        // Liste des tâches
        // Action appelée depuis la vue Index
        public async Task<IActionResult> Index(string texteDesc, int etat)
        {
            ViewBag.Etats = new Dictionary<int, string>()
            {
                { 0, "" },
                { -1, "Non terminée" },
                { 1, "Terminée" }
            };

            // Mémorisation de valeurs de filtres saisies
            ViewBag.texteDesc = texteDesc;
            ViewBag.EtatSelec = etat;

            IQueryable<Tache> taches = _context.Taches;
            ViewData["NbTachesTerminees"] = taches.Where(t => t.Terminee == true).ToList().Count;

            // Si le critère de filtre sur la description est spécifié, on l'applique
            if (!string.IsNullOrEmpty(texteDesc))
                taches = taches.Where(t => t.Description.Contains(texteDesc));

            if (etat != 0)
                taches = taches.Where(t => t.Terminee == (etat > 0));

            return View(await taches.ToListAsync());
        }

        // GET: Taches/Details/5
        // Détail d'une tâche
        // Action appelée depuis la vue Index
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tache = await _context.Taches.SingleOrDefaultAsync(m => m.Id == id);
            if (tache == null) return NotFound();

            return View(tache);
        }

        // GET: Taches/Create
        // Création d'une tâche vide
        // Action appelée depuis la vue Index
        public IActionResult Create()
        {
            return View();
        }

        // POST: Taches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Validation de la tâche saisie par l'utilisateur
        // Action appelée depuis la vue Create
        // Si la tâche est valide, redirection vers l'action Index pour afficher la liste des tâches
        // Sinon, le formulaire de création de tâche reste affiché
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,DateCreation,DateEcheance,Terminee")] Tache tache)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tache);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tache);
        }

        // GET: Taches/Edit/5
        // Edition de la tâche dont l'id est passé en paramètre
        // Action appelée depuis la vue Index
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tache = await _context.Taches.SingleOrDefaultAsync(m => m.Id == id);
            if (tache == null)
            {
                return NotFound();
            }
            return View(tache);
        }

        // POST: Taches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Validation de la tâche éditée par l'utilisateur
        // Action appelée depuis la vue Edit
        // Si la tâche est valide, redirection vers l'action Index pour afficher la liste des tâches
        // Sinon, le formulaire d'édition de la tâche reste affiché
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,DateCreation,DateEcheance,Terminee")] Tache tache)
        {
            if (id != tache.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tache);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TacheExists(tache.Id))
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
            return View(tache);
        }

        // GET: Taches/Delete/5
        // Demande de suppression de la tâche dont l'id est passé en paramètre
        // Action appelée depuis la vue Index
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tache = await _context.Taches.SingleOrDefaultAsync(m => m.Id == id);
            if (tache == null)
            {
                return NotFound();
            }

            return View(tache);
        }

        // POST: Taches/Delete/5
        // Validation de la tâche éditée par l'utilisateur
        // Action appelée depuis la vue Edit
        // Si la tâche est valide, redirection vers l'action Index pour afficher la liste des tâches
        // Sinon, le formulaire d'édition de la tâche reste affiché
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tache = await _context.Taches.SingleOrDefaultAsync(m => m.Id == id);
            _context.Taches.Remove(tache);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TacheExists(int id)
        {
            return _context.Taches.Any(e => e.Id == id);
        }
    }
}
