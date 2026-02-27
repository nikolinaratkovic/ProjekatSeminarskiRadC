using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat.Models
{
    public class Ispit
    {
        public int IspitID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int PredmetID { get; set; }

        [Range(5, 10)]
        public int Ocena { get; set; }

        [Required]
        public DateTime DatumPolaganja { get; set; }

        [Required]
        [MaxLength(50)]
        public string IspitniRok { get; set; }

        [ForeignKey("StudentID")]
        public Student Student { get; set; }

        [ForeignKey("PredmetID")]
        public Predmet Predmet { get; set; }
    }
}
