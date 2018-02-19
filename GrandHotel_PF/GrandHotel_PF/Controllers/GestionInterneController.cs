using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrandHotel_PF.Controllers
{
    [Authorize]
    public class GestionInterneController : Controller
    {
        // Page intermédiaire avant d'accéder à la liste des clients ou à la liste des chambres
        public IActionResult Index()
        {
            return View();
        }
    }
}