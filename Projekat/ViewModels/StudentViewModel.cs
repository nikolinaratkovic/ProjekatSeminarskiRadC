using Projekat.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Projekat.ViewModels
{
    public class StudentViewModel : BaseViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;

                    if (value != null)
                    {
                        Ime = value.Ime;
                        Prezime = value.Prezime;
                        BrojIndeksa = value.BrojIndeksa;
                        GodinaStudija = value.GodinaStudija;
                    }

                    OnPropertyChanged(nameof(SelectedStudent));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _ime;
        public string Ime
        {
            get => _ime;
            set
            {
                _ime = value;
                OnPropertyChanged(nameof(Ime));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _prezime;
        public string Prezime
        {
            get => _prezime;
            set
            {
                _prezime = value;
                OnPropertyChanged(nameof(Prezime));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _brojIndeksa;
        public string BrojIndeksa
        {
            get => _brojIndeksa;
            set
            {
                _brojIndeksa = value;
                OnPropertyChanged(nameof(BrojIndeksa));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _godinaStudija = 1;
        public int GodinaStudija
        {
            get => _godinaStudija;
            set
            {
                _godinaStudija = value;
                OnPropertyChanged(nameof(GodinaStudija));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Ime):
                        if (string.IsNullOrWhiteSpace(Ime))
                            return "Ime je obavezno.";
                        if (Ime.Length > 50)
                            return "Ime ne sme biti duže od 50 karaktera.";
                        break;

                    case nameof(Prezime):
                        if (string.IsNullOrWhiteSpace(Prezime))
                            return "Prezime je obavezno.";
                        if (Prezime.Length > 50)
                            return "Prezime ne sme biti duže od 50 karaktera.";
                        break;

                    case nameof(BrojIndeksa):
                        if (string.IsNullOrWhiteSpace(BrojIndeksa))
                            return "Broj indeksa je obavezan.";
                        if (BrojIndeksa.Length > 20)
                            return "Broj indeksa ne sme biti duži od 20 karaktera.";
                        break;

                    case nameof(GodinaStudija):
                        if (GodinaStudija < 1 || GodinaStudija > 5)
                            return "Godina studija mora biti između 1 i 5.";
                        break;
                }

                return null;
            }
        }

        public StudentViewModel()
        {
            // Test podaci
            Students.Add(new Student
            {
                StudentID = 1,
                Ime = "Nikolina",
                Prezime = "Ratkovic",
                BrojIndeksa = "2026/001",
                GodinaStudija = 3
            });

            Students.Add(new Student
            {
                StudentID = 2,
                Ime = "Marko",
                Prezime = "Petrovic",
                BrojIndeksa = "2026/002",
                GodinaStudija = 2
            });

            AddCommand = new RelayCommand(o => AddStudent(), o => CanSave());
            EditCommand = new RelayCommand(o => EditStudent(), o => SelectedStudent != null && CanSave());
            DeleteCommand = new RelayCommand(o => DeleteStudent(), o => SelectedStudent != null);
        }

        private void AddStudent()
        {
            var noviStudent = new Student
            {
                StudentID = Students.Count > 0 ? Students.Max(s => s.StudentID) + 1 : 1,
                Ime = Ime,
                Prezime = Prezime,
                BrojIndeksa = BrojIndeksa,
                GodinaStudija = GodinaStudija
            };

            Students.Add(noviStudent);
            ResetForm();
        }

        private void EditStudent()
        {
            if (SelectedStudent != null)
            {
                SelectedStudent.Ime = Ime;
                SelectedStudent.Prezime = Prezime;
                SelectedStudent.BrojIndeksa = BrojIndeksa;
                SelectedStudent.GodinaStudija = GodinaStudija;
            }
        }

        private void DeleteStudent()
        {
            if (SelectedStudent != null)
            {
                Students.Remove(SelectedStudent);
                ResetForm();
            }
        }

        private void ResetForm()
        {
            SelectedStudent = null;
            Ime = string.Empty;
            Prezime = string.Empty;
            BrojIndeksa = string.Empty;
            GodinaStudija = 1;
        }

        private bool CanSave()
        {
            return this[nameof(Ime)] == null &&
                   this[nameof(Prezime)] == null &&
                   this[nameof(BrojIndeksa)] == null &&
                   this[nameof(GodinaStudija)] == null;
        }
    }
}