﻿using System;
using System.Collections.Generic;

namespace GrandHotelAPI_PF.Models
{
    public partial class Tarif
    {
        public Tarif()
        {
            TarifChambre = new HashSet<TarifChambre>();
        }

        public string Code { get; set; }
        public DateTime DateDebut { get; set; }
        public decimal Prix { get; set; }

        public ICollection<TarifChambre> TarifChambre { get; set; }
    }
}
