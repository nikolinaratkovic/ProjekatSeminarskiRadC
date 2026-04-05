using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekat.Models
{
    public class Ispit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int IspitID { get; set; }

        private int _studentID;
        [Required]
        public int StudentID
        {
            get => _studentID;
            set
            {
                _studentID = value;
                OnPropertyChanged(nameof(StudentID));
            }
        }

        private int _predmetID;
        [Required]
        public int PredmetID
        {
            get => _predmetID;
            set
            {
                _predmetID = value;
                OnPropertyChanged(nameof(PredmetID));
            }
        }

        private int _ocena;
        [Range(5, 10)]
        public int Ocena
        {
            get => _ocena;
            set
            {
                _ocena = value;
                OnPropertyChanged(nameof(Ocena));
            }
        }

        private DateTime _datumPolaganja;
        [Required]
        public DateTime DatumPolaganja
        {
            get => _datumPolaganja;
            set
            {
                _datumPolaganja = value;
                OnPropertyChanged(nameof(DatumPolaganja));
            }
        }

        private string _ispitniRok;
        [Required]
        [MaxLength(50)]
        public string IspitniRok
        {
            get => _ispitniRok;
            set
            {
                _ispitniRok = value;
                OnPropertyChanged(nameof(IspitniRok));
            }
        }

        [ForeignKey("StudentID")]
        public Student Student { get; set; }

        [ForeignKey("PredmetID")]
        public Predmet Predmet { get; set; }
    }
}