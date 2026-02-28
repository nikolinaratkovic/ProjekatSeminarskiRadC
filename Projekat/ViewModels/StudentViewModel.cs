using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekat.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get { return _selectedStudent; }
            set
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
            }
        }

        private string _ime;
        public string Ime
        {
            get => _ime;
            set { _ime = value; OnPropertyChanged(nameof(Ime)); }
        }

        private string _prezime;
        public string Prezime
        {
            get => _prezime;
            set { _prezime = value; OnPropertyChanged(nameof(Prezime)); }
        }

        private string _brojIndeksa;
        public string BrojIndeksa
        {
            get => _brojIndeksa;
            set { _brojIndeksa = value; OnPropertyChanged(nameof(BrojIndeksa)); }
        }

        private int _godinaStudija;
        public int GodinaStudija
        {
            get => _godinaStudija;
            set { _godinaStudija = value; OnPropertyChanged(nameof(GodinaStudija)); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case nameof(Ime):
                        if (string.IsNullOrWhiteSpace(Ime))
                            result = "Ime je obavezno. ";

                        else if (Ime.Length > 50)
                            result = "Ime ne sme biti duže od 50 karaktera. ";

                        break;

                    case nameof(Prezime):
                        if (string.IsNullOrWhiteSpace(Prezime))
                            result = "Prezime je obavezno. ";

                        else if (Prezime.Length > 50)
                            result = "Prezime ne sme biti duže od 50 karaktera. ";

                        break;

                    case nameof(BrojIndeksa):
                        if (string.IsNullOrWhiteSpace(BrojIndeksa))
                            result = "Broj indeksa je obavezno. ";

                        else if (BrojIndeksa.Length > 20)
                            result = "Broj indeksa ne sme biti duže od 20 karaktera. ";

                        break;

                    case nameof(GodinaStudija):
                        if (GodinaStudija < 1 || GodinaStudija > 5)
                            result = "Godina studija mora biti između 1 i 5. ";

                        break;
                }

                return result;
            }
        }

        public StudentViewModel()
        {
            Students.Add(new Student { StudentID = 1, Ime = "Nikolina", Prezime = "Ratkovic", BrojIndeksa = "2026/001", GodinaStudija = 3 });
            Students.Add(new Student { StudentID = 2, Ime = "Marko", Prezime = "Petrovic", BrojIndeksa = "2026/002", GodinaStudija = 2 });

            AddCommand = new RelayCommand(o => AddStudent(), o => CanSave());
            EditCommand = new RelayCommand(o => EditStudent(), o => SelectedStudent != null && CanSave());
            DeleteCommand = new RelayCommand(o => DeleteStudent(), o => SelectedStudent != null);
        }

        private void AddStudent()
        {
            var noviStudent = new Student
            {
                StudentID = Students.Count > 0 ? Students.Max(s => s.StudentID) + 1 : 1,
                Ime = this.Ime,
                Prezime = this.Prezime,
                BrojIndeksa = this.BrojIndeksa,
                GodinaStudija = this.GodinaStudija
            };

            Students.Add(noviStudent);

            Ime = Prezime = BrojIndeksa = "";
            GodinaStudija = 1;
        }

        private void EditStudent()
        {
            if (SelectedStudent != null)
            {
                SelectedStudent.Ime = Ime;
                SelectedStudent.Prezime = Prezime;
                SelectedStudent.BrojIndeksa = BrojIndeksa;
                SelectedStudent.GodinaStudija = GodinaStudija;

                OnPropertyChanged(nameof(Students));
            }
        }

        private void DeleteStudent()
        {
            if (SelectedStudent != null)
            {
                Students.Remove(SelectedStudent);

                SelectedStudent = null;
                Ime = Prezime = BrojIndeksa = "";
                GodinaStudija = 1;
            }
        }

        private bool CanSave()
        {
            return string.IsNullOrWhiteSpace(this[nameof(Ime)]) &&
                string.IsNullOrWhiteSpace(this[nameof(Prezime)]) &&
                string.IsNullOrWhiteSpace(this[nameof(BrojIndeksa)]) &&
                string.IsNullOrWhiteSpace(this[nameof(GodinaStudija)]);
        }
    }
}
