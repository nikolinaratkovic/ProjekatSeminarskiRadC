using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Projekat.Models
{
    public class Predmet : INotifyPropertyChanged
    {
        private int _predmetID;
        private string _naziv;
        private int _espb;
        private int _semestar;

        public int PredmetID
        {
            get => _predmetID;
            set
            {
                if (_predmetID != value)
                {
                    _predmetID = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required]
        [MaxLength(100)]
        public string Naziv
        {
            get => _naziv;
            set
            {
                if (_naziv != value)
                {
                    _naziv = value;
                    OnPropertyChanged();
                }
            }
        }

        [Range(1, 30)]
        public int ESPB
        {
            get => _espb;
            set
            {
                if (_espb != value)
                {
                    _espb = value;
                    OnPropertyChanged();
                }
            }
        }

        [Range(1, 10)]
        public int Semestar
        {
            get => _semestar;
            set
            {
                if (_semestar != value)
                {
                    _semestar = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICollection<Ispit> Ispiti { get; set; } = new List<Ispit>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}