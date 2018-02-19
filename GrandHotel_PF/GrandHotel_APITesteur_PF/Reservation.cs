using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandHotel_APITesteur_PF
{
    public partial class Reservation
    {
        public short NumChambre { get; set; }
        //[DataType(DataType.Date)]
        public DateTime Jour { get; set; }
        public int IdClient { get; set; }
        public byte NbPersonnes { get; set; }
        public byte HeureArrivee { get; set; }
        public bool? Travail { get; set; }

        public Client IdClientNavigation { get; set; }
        //public Calendrier JourNavigation { get; set; }
        //public Chambre NumChambreNavigation { get; set; }
    }
}
