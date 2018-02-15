using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Models
{
    public class Tache
    {
        public int Id { get; set; }

        [DataType(DataType.MultilineText)]
        [Required, MaxLength(250)]
        public string Description { get; set; }

        [Display(Name = "Date de création")]
        [DataType(DataType.Date)]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Date d'échéance")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:ddd dd MMM yyyy}")]
        public DateTime? DateEcheance { get; set; }

        public bool Terminee { get; set; }
    }
}
