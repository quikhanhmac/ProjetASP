using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_PF.Models
{
    public class ChambreVM
    {
        // Objet appartenant à la classe Chambre
        public Chambre Chambre { get; set; }
        public decimal Prix { get; set; }
        public decimal PrixTotal { get; set; }

        //CodeTarif de la chambre
        public string CodeTarif { get; set; }

        //// Tarif de la chambre à la date du jour
        //public decimal Tarif { get; set; }

        // Etat de la chambre à la date du jour
        [Display(Name = "Etat")]
        public string EtatChambre { get; set; }

        // Date de réservation (à comparer avec la date du jour)
        [DataType(DataType.Date)]
        public DateTime Jour { get; set; }

        // Objet de la classe réservation pour récupérer l'id des clients occupant la chambre à la date du jour
        public Reservation Reservation { get; set; }
    }
}
