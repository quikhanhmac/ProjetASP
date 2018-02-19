using System;
using System.Collections.Generic;

namespace GrandHotelAPI_PF.Models
{
    public partial class TarifChambre
    {
        public short NumChambre { get; set; }
        public string CodeTarif { get; set; }

        public Tarif CodeTarifNavigation { get; set; }
        public Chambre NumChambreNavigation { get; set; }
    }
}
