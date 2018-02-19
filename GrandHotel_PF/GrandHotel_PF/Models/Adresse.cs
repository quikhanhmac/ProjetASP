using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrandHotel_PF.Models
{
    public partial class Adresse
    {
        public int IdClient { get; set; }

        [MaxLength(40)]
        public string Rue { get; set; }

        [MaxLength(40)]
        public string Complement { get; set; }

        [MaxLength(5)]
        public string CodePostal { get; set; }

        [MaxLength(40)]
        public string Ville { get; set; }

        public Client IdClientNavigation { get; set; }
    }
}
