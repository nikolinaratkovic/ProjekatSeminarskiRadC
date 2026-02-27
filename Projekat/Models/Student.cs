using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Ime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Prezime { get; set; }

        [Required]
        [MaxLength(20)]
        public string BrojIndeksa { get; set; }

        [Range(1, 5)]
        public int GodinaStudija { get; set; }

        public ICollection<Ispit> Ispiti { get; set; } = new List<Ispit>();
    }
}
