using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Route("[Controller]")]
    public class UtilitairesController : Controller
    {
        [Route("Calculatrice")]
        [Route("Index")]
        public IActionResult Index()
        {
            var calcul = new Calcul { DateDeb = DateTime.Today };

            return View("Index", calcul);
        }

        [Route("AjouterJours")]
        public IActionResult AjouterJours(Calcul calcul)
        {
            /* ModelState représente l'état de la liaison de modèle après saisie
             des valeurs par l'utilisateur. Il doit être rafraîchi pour que le
             résultat de l'opération s'affiche. On peut forcer ce rafraichissement en
             vidant le ModelState complètement ou seulement la propriété correspondant au résultat*/

            //ModelState.Remove("DateRes");
            var res = calcul.DateDeb.AddDays(calcul.NbJours * calcul.Operation);
            //calcul.DateRes = res;
            ViewBag.DateRes = res;

            return View("Index");
        }
    }
}