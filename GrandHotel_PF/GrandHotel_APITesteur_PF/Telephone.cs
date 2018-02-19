using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandHotel_APITesteur_PF
{
    public partial class Telephone
    {
        public string Numero { get; set; }
        public int IdClient { get; set; }
        public string CodeType { get; set; }
        public bool Pro { get; set; }

        public Client IdClientNavigation { get; set; }
    }
}
