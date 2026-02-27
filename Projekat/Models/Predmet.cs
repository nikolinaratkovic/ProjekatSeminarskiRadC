using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat.Models
{
    public class Predmet
    {
        public int PredmetID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Naziv { get; set; }

        [Range(1, 30)]
        public int ESPB { get; set; }

        [Range(1, 10)]
        public int Semester { get; set; }

        public ICollection<Ispit> Ispiti { get; set; } = new List<Ispit>();
    }
}
