using Projekat.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Projekat.ViewModels
{
    public class IspitViewModel : BaseViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Ispit> Ispiti { get; set; } = new ObservableCollection<Ispit>();
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        public ObservableCollection<Predmet> Predmets { get; set; } = new ObservableCollection<Predmet>();

        private Ispit _selectedIspit;
        public Ispit SelectedIspit
        {
            get => _selectedIspit;
            set
            {
                _selectedIspit = value;
                if (value != null)
                {
                    SelectedStudent = Students.FirstOrDefault(s => s.StudentID == value.StudentID);
                    SelectedPredmet = Predmets.FirstOrDefault(p => p.PredmetID == value.PredmetID);
                    Ocena = value.Ocena;
                    DatumPolaganja = value.DatumPolaganja;
                }
                OnPropertyChanged(nameof(SelectedIspit));
            }
        }

        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        private Predmet _selectedPredmet;
        public Predmet SelectedPredmet
        {
            get => _selectedPredmet;
            set
            {
                _selectedPredmet = value;
                OnPropertyChanged(nameof(SelectedPredmet));
            }
        }

        private int _ocena = 6;
        public int Ocena
        {
            get => _ocena;
            set
            {
                _ocena = value;
                OnPropertyChanged(nameof(Ocena));
            }
        }

        private DateTime _datumPolaganja = DateTime.Today;
        public DateTime DatumPolaganja
        {
            get => _datumPolaganja;
            set
            {
                _datumPolaganja = value;
                OnPropertyChanged(nameof(DatumPolaganja));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public IspitViewModel()
        {
            // Mock studenti i predmeti
            Students.Add(new Student { StudentID = 1, Ime = "Nikolina", Prezime = "R." });
            Students.Add(new Student { StudentID = 2, Ime = "Marko", Prezime = "M." });

            Predmets.Add(new Predmet { PredmetID = 1, Naziv = "Matematika", ESPB = 6, Semestar = 1 });
            Predmets.Add(new Predmet { PredmetID = 2, Naziv = "Fizika", ESPB = 7, Semestar = 1 });

            // Mock ispiti
            Ispiti.Add(new Ispit { IspitID = 1, StudentID = 1, PredmetID = 1, Ocena = 9, DatumPolaganja = DateTime.Today });
            Ispiti.Add(new Ispit { IspitID = 2, StudentID = 2, PredmetID = 2, Ocena = 8, DatumPolaganja = DateTime.Today.AddDays(-7) });

            AddCommand = new RelayCommand(o => DodajIspit(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajIspit(), o => SelectedIspit != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiIspit(), o => SelectedIspit != null);
        }

        private void DodajIspit()
        {
            if (!CanSave()) return;

            var novi = new Ispit
            {
                IspitID = Ispiti.Any() ? Ispiti.Max(i => i.IspitID) + 1 : 1,
                StudentID = SelectedStudent.StudentID,
                PredmetID = SelectedPredmet.PredmetID,
                Ocena = Ocena,
                DatumPolaganja = DatumPolaganja
            };
            Ispiti.Add(novi);
            ResetForm();
        }

        private void AzurirajIspit()
        {
            if (SelectedIspit == null || !CanSave()) return;

            SelectedIspit.StudentID = SelectedStudent.StudentID;
            SelectedIspit.PredmetID = SelectedPredmet.PredmetID;
            SelectedIspit.Ocena = Ocena;
            SelectedIspit.DatumPolaganja = DatumPolaganja;

            ResetForm();
        }

        private void ObrisiIspit()
        {
            if (SelectedIspit == null) return;
            Ispiti.Remove(SelectedIspit);
            ResetForm();
        }

        private void ResetForm()
        {
            SelectedIspit = null;
            SelectedStudent = null;
            SelectedPredmet = null;
            Ocena = 6;
            DatumPolaganja = DateTime.Today;
        }

        private bool CanSave()
        {
            return SelectedStudent != null &&
                   SelectedPredmet != null &&
                   Ocena >= 6 && Ocena <= 10;
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Ocena):
                        if (Ocena < 6 || Ocena > 10)
                            return "Ocena mora biti između 6 i 10.";
                        break;
                }
                return null;
            }
        }
    }
}