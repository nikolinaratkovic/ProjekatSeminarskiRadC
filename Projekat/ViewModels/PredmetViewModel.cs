using Projekat.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

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
            // Test podaci
            Predmets.Add(new Predmet { PredmetID = 1, Naziv = "Programiranje 1", ESPB = 7, Semestar = 4 });
            Predmets.Add(new Predmet { PredmetID = 2, Naziv = "Programiranje 2", ESPB = 8, Semestar = 5 });

            AddCommand = new RelayCommand(o => AddPredmet(), o => CanSave());
            EditCommand = new RelayCommand(o => EditPredmet(), o => SelectedPredmet != null && CanSave());
            DeleteCommand = new RelayCommand(o => DeletePredmet(), o => SelectedPredmet != null);
        }

        private void AddPredmet()
        {
            var noviPredmet = new Predmet
            {
                PredmetID = Predmets.Count > 0 ? Predmets.Max(p => p.PredmetID) + 1 : 1,
                Naziv = Naziv,
                ESPB = ESPB,
                Semestar = Semestar
            };

            Predmets.Add(noviPredmet);
            ClearForm();
        }

        private void EditPredmet()
        {
            if (SelectedPredmet == null) return;

            SelectedPredmet.Naziv = Naziv;
            SelectedPredmet.ESPB = ESPB;
            SelectedPredmet.Semestar = Semestar;

            // NEMA više OnPropertyChanged(nameof(Predmets));
            // jer model sada ima INotifyPropertyChanged
        }

        private void DeletePredmet()
        {
            if (SelectedPredmet == null) return;

            Predmets.Remove(SelectedPredmet);
            ClearForm();
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