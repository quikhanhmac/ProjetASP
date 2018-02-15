using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrandHotel_PF.Models
{
    public partial class Client
    {
        public Client()
        {
            Facture = new HashSet<Facture>();
            Reservation = new HashSet<Reservation>();
            Telephone = new HashSet<Telephone>();
        }

        public int Id { get; set; }
        //[Required]
        [MaxLength(4)]
        public string Civilite { get; set; }
        //[Required]
        public string Nom { get; set; }
        //[Required]
        public string Prenom { get; set; }
        public string Email { get; set; }
        public bool CarteFidelite { get; set; }
        public string Societe { get; set; }

        public Adresse Adresse { get; set; }
        public ICollection<Facture> Facture { get; set; }
        public ICollection<Reservation> Reservation { get; set; }

        //[Required]
        //[Required, RegularExpression(@"^[0-9]{10}$")]
        public ICollection<Telephone> Telephone { get; set; }
    }
}
