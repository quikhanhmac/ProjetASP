﻿using System;
using System.Collections.Generic;

namespace GrandHotel_PF.Models
{
    public partial class Reservation
    {
        public short NumChambre { get; set; }
        public DateTime Jour { get; set; }
        public int IdClient { get; set; }
        public byte NbPersonnes { get; set; }
        public byte HeureArrivee { get; set; }
        public bool? Travail { get; set; }

        public Client IdClientNavigation { get; set; }
        public Calendrier JourNavigation { get; set; }
        public Chambre NumChambreNavigation { get; set; }
    }
}
