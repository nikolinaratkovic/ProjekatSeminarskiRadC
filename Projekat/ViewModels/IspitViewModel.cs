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

        private readonly IspitService _ispitService;
        private readonly StudentService _studentService;
        private readonly PredmetService _predmetService;

        public ObservableCollection<Ispit> Ispiti { get; set; } = new ObservableCollection<Ispit>();
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        public ObservableCollection<Predmet> Predmets { get; set; } = new ObservableCollection<Predmet>();

        public ObservableCollection<string> IspitniRokoviLista { get; set; } = new()
        {
            "Januar 2023", "Jun 2023", "Jul 2023", "Septembar 2023", "Oktobar 2023",
            "Januar 2024", "Jun 2024", "Jul 2024", "Septembar 2024", "Oktobar 2024",
            "Januar 2025", "Jun 2025", "Jul 2025", "Septembar 2025", "Oktobar 2025",
            "Januar 2026", "Jun 2026", "Jul 2026", "Septembar 2026", "Oktobar 2026"
        };

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
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set { _selectedStudent = value; OnPropertyChanged(nameof(SelectedStudent)); CommandManager.InvalidateRequerySuggested(); }
        }

        private Predmet _selectedPredmet;
        public Predmet SelectedPredmet
        {
            get => _selectedPredmet;
            set { _selectedPredmet = value; OnPropertyChanged(nameof(SelectedPredmet)); CommandManager.InvalidateRequerySuggested(); }
        }

        private int _ocena = 6;
        public int Ocena
        {
            get => _ocena;
            set { _ocena = value; OnPropertyChanged(nameof(Ocena)); CommandManager.InvalidateRequerySuggested(); }
        }

        private DateTime _datumPolaganja = DateTime.Today;
        public DateTime DatumPolaganja
        {
            get => _datumPolaganja;
            set { _datumPolaganja = value; OnPropertyChanged(nameof(DatumPolaganja)); CommandManager.InvalidateRequerySuggested(); }
        }

        private string _ispitniRok = string.Empty;
        public string IspitniRok
        {
            get => _ispitniRok;
            set { _ispitniRok = value; OnPropertyChanged(nameof(IspitniRok)); CommandManager.InvalidateRequerySuggested(); }
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
                    case nameof(SelectedStudent):
                        if (SelectedStudent == null) return "Student je obavezan.";
                        break;
                    case nameof(SelectedPredmet):
                        if (SelectedPredmet == null) return "Predmet je obavezan.";
                        break;
                    case nameof(Ocena):
                        if (Ocena < 5 || Ocena > 10) return "Ocena mora biti između 5 i 10.";
                        break;
                    case nameof(IspitniRok):
                        if (string.IsNullOrWhiteSpace(IspitniRok)) return "Ispitni rok je obavezan.";
                        if (IspitniRok.Length > 50) return "Maksimalno 50 karaktera.";
                        break;
                    case nameof(DatumPolaganja):
                        if (DatumPolaganja == default) return "Datum polaganja je obavezan.";
                        if (DatumPolaganja > DateTime.Today) return "Datum polaganja ne može biti u budućnosti.";
                        break;
                }
                return null;
            }
        }

        public IspitViewModel()
        {
            var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(Config.ConnectionString)
                    .Options
            );

            _ispitService = new IspitService(context);
            _studentService = new StudentService(context);
            _predmetService = new PredmetService(context);

            UcitajPodatkeAsync();

            AddCommand = new RelayCommand(o => DodajIspitAsync(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajIspitAsync(), o => SelectedIspit != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiIspitAsync(), o => SelectedIspit != null);
        }

        private async void UcitajPodatkeAsync()
        {
            try
            {
                Students.Clear();
                foreach (var s in await _studentService.UcitajSveStudenteAsync())
                    Students.Add(s);

                Predmets.Clear();
                foreach (var p in await _predmetService.UcitajSvePredmete())
                    Predmets.Add(p);

                Ispiti.Clear();
                foreach (var i in await _ispitService.UcitajSveIspite())
                    Ispiti.Add(i);

                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju podataka: {ex.Message}", "Greška");
            }
        }

        private async void DodajIspitAsync()
        {
            ValidateAllProperties();
            if (!CanSave())
            {
                System.Windows.MessageBox.Show("Popunite sva polja ispravno.", "Validacija");
                return;
            }

            try
            {
                var novi = new Ispit
                {
                    StudentID = SelectedStudent.StudentID,
                    PredmetID = SelectedPredmet.PredmetID,
                    Ocena = Ocena,
                    DatumPolaganja = DatumPolaganja,
                    IspitniRok = IspitniRok
                };

                var dodani = await _ispitService.DodajIspit(novi);
                Ispiti.Add(dodani);

                System.Windows.MessageBox.Show("Ispit uspešno dodat.", "Uspeh");
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri dodavanju: {ex.Message}", "Greška");
            }
        }

        private async void AzurirajIspitAsync()
        {
            if (SelectedIspit == null) return;

            ValidateAllProperties();
            if (!CanSave())
            {
                System.Windows.MessageBox.Show("Popunite sva polja ispravno.", "Validacija");
                return;
            }

            try
            {
                var noviPodaci = new Ispit
                {
                    StudentID = SelectedStudent.StudentID,
                    PredmetID = SelectedPredmet.PredmetID,
                    Ocena = Ocena,
                    DatumPolaganja = DatumPolaganja,
                    IspitniRok = IspitniRok
                };

                await _ispitService.AzurirajIspit(SelectedIspit.IspitID, noviPodaci);

                SelectedIspit.StudentID = SelectedStudent.StudentID;
                SelectedIspit.PredmetID = SelectedPredmet.PredmetID;
                SelectedIspit.Ocena = Ocena;
                SelectedIspit.DatumPolaganja = DatumPolaganja;
                SelectedIspit.IspitniRok = IspitniRok;

                System.Windows.MessageBox.Show("Ispit uspešno ažuriran.", "Uspeh");
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri ažuriranju: {ex.Message}", "Greška");
            }
        }

        private async void ObrisiIspitAsync()
        {
            if (SelectedIspit == null) return;

            var rezultat = System.Windows.MessageBox.Show(
                $"Da li želite da obrišete ispit?",
                "Potvrda", System.Windows.MessageBoxButton.YesNo);

            if (rezultat != System.Windows.MessageBoxResult.Yes) return;

            try
            {
                await _ispitService.ObrisiIspit(SelectedIspit.IspitID);
                Ispiti.Remove(SelectedIspit);
                System.Windows.MessageBox.Show("Ispit uspešno obrisan.", "Uspeh");
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška");
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

            ValidateAllProperties();
        }

        private bool CanSave()
        {
            return this[nameof(SelectedStudent)] == null &&
                   this[nameof(SelectedPredmet)] == null &&
                   this[nameof(Ocena)] == null &&
                   this[nameof(IspitniRok)] == null &&
                   this[nameof(DatumPolaganja)] == null;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ValidateAllProperties()
        {
            OnPropertyChanged(nameof(SelectedStudent));
            OnPropertyChanged(nameof(SelectedPredmet));
            OnPropertyChanged(nameof(Ocena));
            OnPropertyChanged(nameof(IspitniRok));
            OnPropertyChanged(nameof(DatumPolaganja));
        }
    }
}