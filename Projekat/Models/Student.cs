using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Projekat.Models
{
    public class Student : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int StudentID { get; set; }

        private string _ime;
        [Required]
        [MaxLength(50)]
        public string Ime
        {
            get => _ime;
            set
            {
                _ime = value;
                OnPropertyChanged(nameof(Ime));
            }
        }

        private string _prezime;
        [Required]
        [MaxLength(50)]
        public string Prezime
        {
            get => _prezime;
            set
            {
                _prezime = value;
                OnPropertyChanged(nameof(Prezime));
            }
        }

        private string _brojIndeksa;
        [Required]
        [MaxLength(20)]
        public string BrojIndeksa
        {
            get => _brojIndeksa;
            set
            {
                _brojIndeksa = value;
                OnPropertyChanged(nameof(BrojIndeksa));
            }
        }

        private int _godinaStudija;
        [Range(1, 5)]
        public int GodinaStudija
        {
            get => _godinaStudija;
            set
            {
                _godinaStudija = value;
                OnPropertyChanged(nameof(GodinaStudija));
            }
        }

        public ICollection<Ispit> Ispiti { get; set; } = new List<Ispit>();

        public string FullDisplay => $"{Ime} {Prezime} ({BrojIndeksa})";
    }
}