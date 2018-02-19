using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandHotel_APITesteur_PF
{
    public partial class Adresse
    {
        public int IdClient { get; set; }
        public string Rue { get; set; }
        public string Complement { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }

        public Client IdClientNavigation { get; set; }
    }
}
