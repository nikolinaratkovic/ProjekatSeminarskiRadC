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

        private readonly PredmetService _predmetService;

        public ObservableCollection<Predmet> Predmets { get; set; } = new ObservableCollection<Predmet>();

        public ObservableCollection<int> ESPBOptions { get; } = new ObservableCollection<int>(Enumerable.Range(1, 30));
        public ObservableCollection<int> SemestarOptions { get; } = new ObservableCollection<int>(Enumerable.Range(1, 10));

        private Predmet _selectedPredmet;
        public Predmet SelectedPredmet
        {
            get => _selectedPredmet;
            set
            {
                if (_selectedPredmet != value)
                {
                    _selectedPredmet = value;

                    if (value != null)
                    {
                        Naziv = value.Naziv;
                        ESPB = value.ESPB;
                        Semestar = value.Semestar;
                    }

                    OnPropertyChanged(nameof(SelectedPredmet));
                    CommandManager.InvalidateRequerySuggested();
                }
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
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int? _espb;
        public int? ESPB
        {
            get => _espb;
            set
            {
                _espb = value;
                OnPropertyChanged(nameof(ESPB));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int? _semestar;
        public int? Semestar
        {
            get => _semestar;
            set
            {
                _semestar = value;
                OnPropertyChanged(nameof(Semestar));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public PredmetViewModel()
        {
            var context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(Config.ConnectionString)
                    .Options
            );

            _predmetService = new PredmetService(context);

            UcitajPredmeteAsync();

            AddCommand = new RelayCommand(o => DodajPredmetAsync(), o => CanSave());
            EditCommand = new RelayCommand(o => AzurirajPredmetAsync(), o => SelectedPredmet != null && CanSave());
            DeleteCommand = new RelayCommand(o => ObrisiPredmetAsync(), o => SelectedPredmet != null);
        }

        private async void UcitajPredmeteAsync()
        {
            try
            {
                var predmeti = await _predmetService.UcitajSvePredmete();
                Predmets.Clear();

                foreach (var predmet in predmeti)
                    Predmets.Add(predmet);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju: {ex.Message}", "Greška");
            }
        }

        private async void DodajPredmetAsync()
        {
            ValidateAllProperties();

            if (!CanSave())
            {
                System.Windows.MessageBox.Show("Popunite sva polja ispravno.", "Validacija");
                return;
            }

            try
            {
                var noviPredmet = new Predmet
                {
                    Naziv = Naziv,
                    ESPB = ESPB.Value,
                    Semestar = Semestar.Value
                };

                var dodani = await _predmetService.DodajPredmet(noviPredmet);
                Predmets.Add(dodani);

                System.Windows.MessageBox.Show($"Predmet {dodani.Naziv} uspešno dodat.", "Uspeh");
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri dodavanju: {ex.Message}", "Greška");
            }
        }

        private async void AzurirajPredmetAsync()
        {
            if (SelectedPredmet == null) return;

            ValidateAllProperties();

            if (!CanSave())
            {
                System.Windows.MessageBox.Show("Popunite sva polja ispravno.", "Validacija");
                return;
            }

            try
            {
                var noviPodaci = new Predmet
                {
                    Naziv = Naziv,
                    ESPB = ESPB.Value,
                    Semestar = Semestar.Value
                };

                await _predmetService.AzurirajPredmet(SelectedPredmet.PredmetID, noviPodaci);

                SelectedPredmet.Naziv = Naziv;
                SelectedPredmet.ESPB = ESPB.Value;
                SelectedPredmet.Semestar = Semestar.Value;

                System.Windows.MessageBox.Show("Predmet uspešno ažuriran.", "Uspeh");
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri ažuriranju: {ex.Message}", "Greška");
            }
        }

        private async void ObrisiPredmetAsync()
        {
            if (SelectedPredmet == null) return;

            var rezultat = System.Windows.MessageBox.Show(
                $"Da li želite da obrišete predmet {SelectedPredmet.Naziv}?",
                "Potvrda",
                System.Windows.MessageBoxButton.YesNo);

            if (rezultat != System.Windows.MessageBoxResult.Yes) return;

            try
            {
                await _predmetService.ObrisiPredmet(SelectedPredmet.PredmetID);
                Predmets.Remove(SelectedPredmet);

                System.Windows.MessageBox.Show("Predmet uspešno obrisan.", "Uspeh");
                ResetForm();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri brisanju: {ex.Message}", "Greška");
            }
        }

        private void ResetForm()
        {
            SelectedPredmet = null;
            Naziv = string.Empty;
            ESPB = null;
            Semestar = null;

            ValidateAllProperties();
        }

        private bool CanSave()
        {
            return this[nameof(Naziv)] == null &&
                   this[nameof(ESPB)] == null &&
                   this[nameof(Semestar)] == null;
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
                            return "Max 100 karaktera.";
                        break;

                    case nameof(ESPB):
                        if (ESPB == null)
                            return "ESPB je obavezan.";
                        if (ESPB < 1 || ESPB > 30)
                            return "1-30.";
                        break;

                    case nameof(Semestar):
                        if (Semestar == null)
                            return "Semestar je obavezan.";
                        if (Semestar < 1 || Semestar > 10)
                            return "1-10.";
                        break;
                }

                return null;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ValidateAllProperties()
        {
            OnPropertyChanged(nameof(Naziv));
            OnPropertyChanged(nameof(ESPB));
            OnPropertyChanged(nameof(Semestar));
        }
    }
}