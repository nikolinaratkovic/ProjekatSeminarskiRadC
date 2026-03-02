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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly StudentService _studentService;

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
            
            var context = new ApplicationDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Server=Kecman03pc\\SQLEXPRESS;Database=StudentPerformanceDB;Trusted_Connection=True;TrustServerCertificate=True;")
                    .Options
            );
            _studentService = new StudentService(context);

            UcitajStudente();

            AddCommand = new RelayCommand(o => DodajStudenta(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajStudenta(), o => SelectedStudent != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiStudenta(), o => SelectedStudent != null);
        }

        private async void UcitajStudente()
        {
            try
            {
                var studenti = await _studentService.ucitajSveStudente();
                Students.Clear();
                foreach (var student in studenti)
                {
                    Students.Add(student);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju studenata: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void DodajStudenta()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Ime) || string.IsNullOrWhiteSpace(Prezime) || string.IsNullOrWhiteSpace(BrojIndeksa))
                {
                    System.Windows.MessageBox.Show("Sva polja su obavezna.", "Validacija", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                var noviStudent = new Student
                {
                    Ime = Ime,
                    Prezime = Prezime,
                    BrojIndeksa = BrojIndeksa,
                    GodinaStudija = GodinaStudija
                };

                var dodaniStudent = await _studentService.DodajStudenta(noviStudent);
                Students.Add(dodaniStudent);

                System.Windows.MessageBox.Show($"Student {dodaniStudent.Ime} {dodaniStudent.Prezime} je uspešno dodan.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri dodavanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void AzurirajStudenta()
        {
            try
            {
                if (SelectedStudent == null)
                    return;

                var azuriraniPodaci = new Student
                {
                    Ime = Ime,
                    Prezime = Prezime,
                    BrojIndeksa = BrojIndeksa,
                    GodinaStudija = GodinaStudija
                };

                await _studentService.AzurirajStudenta(SelectedStudent.StudentID, azuriraniPodaci);

                SelectedStudent.Ime = Ime;
                SelectedStudent.Prezime = Prezime;
                SelectedStudent.BrojIndeksa = BrojIndeksa;
                SelectedStudent.GodinaStudija = GodinaStudija;

                System.Windows.MessageBox.Show("Student je uspešno ažuriran.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri ažuriranju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void ObrisiStudenta()
        {
            try
            {
                if (SelectedStudent == null)
                    return;

                var rezultat = System.Windows.MessageBox.Show(
                    $"Sigurno želite da obrišete studenta {SelectedStudent.Ime} {SelectedStudent.Prezime}?",
                    "Potvrda brisanja",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (rezultat != System.Windows.MessageBoxResult.Yes)
                    return;

                await _studentService.ObrisiStudenta(SelectedStudent.StudentID);
                Students.Remove(SelectedStudent);

                System.Windows.MessageBox.Show("Student je uspešno obrisan.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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