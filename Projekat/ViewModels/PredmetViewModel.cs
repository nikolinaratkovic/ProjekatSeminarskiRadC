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
    public class PredmetViewModel : BaseViewModel, INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested();
        }

        private readonly PredmetService _predmetService;

        public ObservableCollection<Predmet> Predmets { get; set; } = new ObservableCollection<Predmet>();

        private Predmet _selectedPredmet;
        public Predmet SelectedPredmet
        {
            get => _selectedPredmet;
            set
            {
                _selectedPredmet = value;

                if (value != null)
                {
                    Naziv = value.Naziv;
                    ESPB = value.ESPB;
                    Semestar = value.Semestar;
                }

                OnPropertyChanged(nameof(SelectedPredmet));
            }
        }

        private string _naziv;
        public string Naziv
        {
            get => _naziv;
            set
            {
                _naziv = value;
                OnPropertyChanged(nameof(Naziv));
            }
        }

        private int _espb;
        public int ESPB
        {
            get => _espb;
            set
            {
                _espb = value;
                OnPropertyChanged(nameof(ESPB));
            }
        }

        private int _semestar;
        public int Semestar
        {
            get => _semestar;
            set
            {
                _semestar = value;
                OnPropertyChanged(nameof(Semestar));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public PredmetViewModel()
        {
            
            var context = new ApplicationDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Server=Kecman03pc\\SQLEXPRESS;Database=StudentPerformanceDB;Trusted_Connection=True;TrustServerCertificate=True;")
                    .Options
            );
            _predmetService = new PredmetService(context);

            UcitajPredmete();

            AddCommand = new RelayCommand(o => DodajPredmet(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajPredmet(), o => SelectedPredmet != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiPredmet(), o => SelectedPredmet != null);
        }

        private async void UcitajPredmete()
        {
            try
            {
                var predmeti = await _predmetService.ucitajSvePredmete();
                Predmets.Clear();
                foreach (var predmet in predmeti)
                {
                    Predmets.Add(predmet);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju predmeta: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void DodajPredmet()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Naziv) || ESPB == 0 || Semestar == 0)
                {
                    System.Windows.MessageBox.Show("Sva polja su obavezna.", "Validacija", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                var noviPredmet = new Predmet
                {
                    Naziv = Naziv,
                    ESPB = ESPB,
                    Semestar = Semestar
                };

                var dodaniPredmet = await _predmetService.dodajPredmet(noviPredmet);
                Predmets.Add(dodaniPredmet);

                System.Windows.MessageBox.Show($"Predmet {dodaniPredmet.Naziv} je uspešno dodan.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri dodavanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void AzurirajPredmet()
        {
            try
            {
                if (SelectedPredmet == null)
                    return;

                var azuriraniPodaci = new Predmet
                {
                    Naziv = Naziv,
                    ESPB = ESPB,
                    Semestar = Semestar
                };

                await _predmetService.azurirajPredmet(SelectedPredmet.PredmetID, azuriraniPodaci);

                SelectedPredmet.Naziv = Naziv;
                SelectedPredmet.ESPB = ESPB;
                SelectedPredmet.Semestar = Semestar;

                System.Windows.MessageBox.Show("Predmet je uspešno ažuriran.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri ažuriranju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void ObrisiPredmet()
        {
            try
            {
                if (SelectedPredmet == null)
                    return;

                var rezultat = System.Windows.MessageBox.Show(
                    $"Sigurno želite da obrišete predmet {SelectedPredmet.Naziv}?",
                    "Potvrda brisanja",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (rezultat != System.Windows.MessageBoxResult.Yes)
                    return;

                await _predmetService.obrisiPredmet(SelectedPredmet.PredmetID);
                Predmets.Remove(SelectedPredmet);

                System.Windows.MessageBox.Show("Predmet je uspešno obrisan.", "Uspešno", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            SelectedPredmet = null;
            Naziv = string.Empty;
            ESPB = 0;
            Semestar = 0;
        }

        private bool CanSave()
        {
            return string.IsNullOrWhiteSpace(this[nameof(Naziv)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ESPB)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(Semestar)]);
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Naziv):
                        if (string.IsNullOrWhiteSpace(Naziv))
                            return "Naziv je obavezan.";
                        if (Naziv.Length > 100)
                            return "Naziv ne sme biti duži od 100 karaktera.";
                        break;

                    case nameof(ESPB):
                        if (ESPB < 1 || ESPB > 30)
                            return "ESPB mora biti između 1 i 30.";
                        break;

                    case nameof(Semestar):
                        if (Semestar < 1 || Semestar > 10)
                            return "Semestar mora biti između 1 i 10.";
                        break;
                }

                return null;
            }
        }
    }
}