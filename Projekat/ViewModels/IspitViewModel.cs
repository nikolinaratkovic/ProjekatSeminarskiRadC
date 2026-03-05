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
    public class IspitViewModel : BaseViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested();
        }

        private readonly IspitService _ispitService;
        private readonly StudentService _studentService;
        private readonly PredmetService _predmetService;

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
                    IspitniRok = value.IspitniRok;
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

        private string _ispitniRok = string.Empty;
        public string IspitniRok
        {
            get => _ispitniRok;
            set
            {
                _ispitniRok = value;
                OnPropertyChanged(nameof(IspitniRok));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public IspitViewModel()
        {
            var context = new ApplicationDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Server=Kecman03pc\\SQLEXPRESS;Database=StudentPerformanceDB;Trusted_Connection=True;TrustServerCertificate=True;")
                    .Options
            );

            _ispitService = new IspitService(context);
            _studentService = new StudentService(context);
            _predmetService = new PredmetService(context);

            UcitajPodatke();

            AddCommand = new RelayCommand(o => DodajIspit(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajIspit(), o => SelectedIspit != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiIspit(), o => SelectedIspit != null);
        }
        private async void UcitajPodatke()
        {
            try
            {
                var studenti = await _studentService.ucitajSveStudente();
                Students.Clear();
                foreach (var student in studenti)
                {
                    Students.Add(student);
                }

                var predmeti = await _predmetService.ucitajSvePredmete();
                Predmets.Clear();
                foreach (var predmet in predmeti)
                {
                    Predmets.Add(predmet);
                }

                var ispiti = await _ispitService.ucitajSveIspite();
                Ispiti.Clear();
                foreach (var ispit in ispiti)
                {
                    Ispiti.Add(ispit);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void DodajIspit()
        {
            try
            {
                if (!CanSave())
                {
                    System.Windows.MessageBox.Show("Molimo popunite sva polja ispravno.", "Validacija", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                var noviIspit = new Ispit
                {
                    StudentID = SelectedStudent.StudentID,
                    PredmetID = SelectedPredmet.PredmetID,
                    Ocena = Ocena,
                    DatumPolaganja = DatumPolaganja,
                    IspitniRok = IspitniRok
                };

                var dodaniIspit = await _ispitService.dodajIspit(noviIspit);
                Ispiti.Add(dodaniIspit);

                System.Windows.MessageBox.Show($"Ispit je uspešno dodan.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri dodavanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void AzurirajIspit()
        {
            try
            {
                if (SelectedIspit == null || !CanSave())
                    return;

                var azuriraniPodaci = new Ispit
                {
                    StudentID = SelectedStudent.StudentID,
                    PredmetID = SelectedPredmet.PredmetID,
                    Ocena = Ocena,
                    DatumPolaganja = DatumPolaganja,
                    IspitniRok = IspitniRok
                };

                await _ispitService.azurirajIspit(SelectedIspit.IspitID, azuriraniPodaci);

                SelectedIspit.StudentID = SelectedStudent.StudentID;
                SelectedIspit.PredmetID = SelectedPredmet.PredmetID;
                SelectedIspit.Ocena = Ocena;
                SelectedIspit.DatumPolaganja = DatumPolaganja;
                SelectedIspit.IspitniRok = IspitniRok;

                System.Windows.MessageBox.Show("Ispit je uspešno ažuriran.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri ažuriranju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void ObrisiIspit()
        {
            try
            {
                if (SelectedIspit == null)
                    return;

                var rezultat = System.Windows.MessageBox.Show(
                    $"Sigurno želite da obrišete ispit?",
                    "Potvrda brisanja",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (rezultat != System.Windows.MessageBoxResult.Yes)
                    return;

                await _ispitService.obrisiIspit(SelectedIspit.IspitID);
                Ispiti.Remove(SelectedIspit);

                System.Windows.MessageBox.Show("Ispit je uspešno obrisan.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ResetForm()
        {
            SelectedIspit = null;
            SelectedStudent = null;
            SelectedPredmet = null;
            Ocena = 6;
            DatumPolaganja = DateTime.Today;
            IspitniRok = string.Empty;
        }

        private bool CanSave()
        {
            return SelectedStudent != null &&
                   SelectedPredmet != null &&
                   !string.IsNullOrWhiteSpace(IspitniRok) &&
                   Ocena >= 5 && Ocena <= 10;
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Ocena):
                        if (Ocena < 5 || Ocena > 10)
                            return "Ocena mora biti između 5 i 10.";
                        break;

                    case nameof(IspitniRok):
                        if (string.IsNullOrWhiteSpace(IspitniRok))
                            return "Ispitni rok je obavezan.";
                        if (IspitniRok.Length > 50)
                            return "Ispitni rok ne sme biti duži od 50 karaktera.";
                        break;
                }
                return null;
            }
        }
    }
}