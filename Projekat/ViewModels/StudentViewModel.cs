using Projekat.Data;
using Projekat.Models;
using Projekat.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace Projekat.ViewModels
{
    public class StudentViewModel : BaseViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly StudentService _studentService;

        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        public ObservableCollection<int> Godine { get; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

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
            set { _ime = value; OnPropertyChanged(nameof(Ime)); CommandManager.InvalidateRequerySuggested(); }
        }

        private string _prezime;
        public string Prezime
        {
            get => _prezime;
            set { _prezime = value; OnPropertyChanged(nameof(Prezime)); CommandManager.InvalidateRequerySuggested(); }
        }

        private string _brojIndeksa;
        public string BrojIndeksa
        {
            get => _brojIndeksa;
            set { _brojIndeksa = value; OnPropertyChanged(nameof(BrojIndeksa)); CommandManager.InvalidateRequerySuggested(); }
        }

        private int _godinaStudija = 1;
        public int GodinaStudija
        {
            get => _godinaStudija;
            set { _godinaStudija = value; OnPropertyChanged(nameof(GodinaStudija)); CommandManager.InvalidateRequerySuggested(); }
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
                        if (string.IsNullOrWhiteSpace(Ime)) return "Ime je obavezno.";
                        if (Ime.Length > 50) return "Ime ne sme biti duže od 50 karaktera.";
                        break;
                    case nameof(Prezime):
                        if (string.IsNullOrWhiteSpace(Prezime)) return "Prezime je obavezno.";
                        if (Prezime.Length > 50) return "Prezime ne sme biti duže od 50 karaktera.";
                        break;
                    case nameof(BrojIndeksa):
                        if (string.IsNullOrWhiteSpace(BrojIndeksa)) return "Broj indeksa je obavezan.";
                        if (BrojIndeksa.Length > 20) return "Broj indeksa ne sme biti duži od 20 karaktera.";
                        break;
                    case nameof(GodinaStudija):
                        if (GodinaStudija < 1 || GodinaStudija > 5) return "Godina studija mora biti između 1 i 5.";
                        break;
                }
                return null;
            }
        }

        public StudentViewModel()
        {
            var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(Config.ConnectionString)
                    .Options
            );

            _studentService = new StudentService(context);

            UcitajStudenteAsync();

            AddCommand = new RelayCommand(o => DodajStudentaAsync(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajStudentaAsync(), o => SelectedStudent != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiStudentaAsync(), o => SelectedStudent != null);
        }

        private async void UcitajStudenteAsync()
        {
            try
            {
                var studenti = await _studentService.UcitajSveStudenteAsync();
                Students.Clear();
                foreach (var student in studenti)
                    Students.Add(student);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void DodajStudentaAsync()
        {
            ValidateAllProperties();

            if (!CanSave())
            {
                System.Windows.MessageBox.Show("Popunite sva polja ispravno.", "Validacija", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                var noviStudent = new Student
                {
                    Ime = Ime,
                    Prezime = Prezime,
                    BrojIndeksa = BrojIndeksa,
                    GodinaStudija = GodinaStudija
                };

                var dodani = await _studentService.DodajStudentaAsync(noviStudent);
                Students.Add(dodani);
                System.Windows.MessageBox.Show($"Student {dodani.Ime} {dodani.Prezime} je uspešno dodat.", "Uspeh", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri dodavanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void AzurirajStudentaAsync()
        {
            if (SelectedStudent == null) return;

            ValidateAllProperties();
            if (!CanSave())
            {
                System.Windows.MessageBox.Show("Popunite sva polja ispravno.", "Validacija",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                var noviPodaci = new Student
                {
                    Ime = Ime,
                    Prezime = Prezime,
                    BrojIndeksa = BrojIndeksa,
                    GodinaStudija = GodinaStudija
                };

                await _studentService.AzurirajStudentaAsync(SelectedStudent.StudentID, noviPodaci);

                SelectedStudent.Ime = Ime;
                SelectedStudent.Prezime = Prezime;
                SelectedStudent.BrojIndeksa = BrojIndeksa;
                SelectedStudent.GodinaStudija = GodinaStudija;

                System.Windows.MessageBox.Show("Student uspešno ažuriran.", "Uspeh", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri ažuriranju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void ObrisiStudentaAsync()
        {
            if (SelectedStudent == null) return;

            var rezultat = System.Windows.MessageBox.Show(
                $"Da li želite da obrišete studenta {SelectedStudent.Ime} {SelectedStudent.Prezime}?",
                "Potvrda brisanja", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (rezultat != System.Windows.MessageBoxResult.Yes) return;

            try
            {
                await _studentService.ObrisiStudentaAsync(SelectedStudent.StudentID);
                Students.Remove(SelectedStudent);
                System.Windows.MessageBox.Show("Student uspešno obrisan.", "Uspeh", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ResetForm()
        {
            SelectedStudent = null;
            Ime = string.Empty;
            Prezime = string.Empty;
            BrojIndeksa = string.Empty;
            GodinaStudija = 1;

            ValidateAllProperties();
        }

        private bool CanSave()
        {
            return this[nameof(Ime)] == null &&
                   this[nameof(Prezime)] == null &&
                   this[nameof(BrojIndeksa)] == null &&
                   this[nameof(GodinaStudija)] == null;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ValidateAllProperties()
        {
            OnPropertyChanged(nameof(Ime));
            OnPropertyChanged(nameof(Prezime));
            OnPropertyChanged(nameof(BrojIndeksa));
            OnPropertyChanged(nameof(GodinaStudija));
        }
    }
}