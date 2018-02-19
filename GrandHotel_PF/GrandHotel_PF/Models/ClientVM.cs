using GrandHotel_PF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_PF.Models
{
    public class ClientVM
    {

        public Client Client { get; set; }

        // Nombre de réservations pour un client donné
        [Display(Name = "Réservations totales")]
        public int NbReservations { get; set; }


        [Display(Name = "Nombre de réservations en cours")]
        public int? ReservationEnCours { get; set; }

        // OUI: si réservation en cours - sinon:NON
        [Display(Name = "Rés. en cours ")]
        public string ResEnCours { get; set; }
    }
}
