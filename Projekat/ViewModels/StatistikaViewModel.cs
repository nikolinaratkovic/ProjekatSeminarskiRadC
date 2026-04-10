using Projekat.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Projekat.ViewModels
{
    public class StatistikaViewModel : BaseViewModel
    {
        private readonly IspitService _ispitService;
        private readonly PredmetService _predmetService;
        private readonly StudentService _studentService;

        public ObservableCollection<string> NaziviPredmeta { get; set; } = new();
        public ObservableCollection<double> ProsecneOcenePredmeta { get; set; } = new();
        public ObservableCollection<string> IspitniRokovi { get; set; } = new();
        public ObservableCollection<int> BrojIspitaPoRokovima { get; set; } = new();

        public ObservableCollection<double> TrendStudenta { get; set; } = new();

        public ObservableCollection<int> Ocene { get; set; } = new();
        public ObservableCollection<int> BrojOcena { get; set; } = new();

        public StatistikaViewModel(
            IspitService ispitService,
            PredmetService predmetService,
            StudentService studentService)
        {
            _ispitService = ispitService;
            _predmetService = predmetService;
            _studentService = studentService;
        }

        public async Task UcitajStatistikuAsync()
        {
            try
            {
                var predmeti = await _predmetService.UcitajSvePredmete();

                var nazivi = new ObservableCollection<string>();
                var proseci = new ObservableCollection<double>();

                foreach (var predmet in predmeti)
                {
                    double prosek = await _ispitService
                        .IzracunajProsecnuOcenuPredmeta(predmet.PredmetID);

                    nazivi.Add(predmet.Naziv);
                    proseci.Add(prosek);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    NaziviPredmeta.Clear();
                    ProsecneOcenePredmeta.Clear();

                    foreach (var n in nazivi)
                        NaziviPredmeta.Add(n);

                    foreach (var p in proseci)
                        ProsecneOcenePredmeta.Add(p);
                });

                var studenti = await _studentService.UcitajSveStudenteAsync();
                var trend = new ObservableCollection<double>();

                foreach (var student in studenti)
                {
                    double prosek = await _ispitService
                        .IzracunajProsecnuOcenuStudenta(student.StudentID);

                    trend.Add(prosek);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    TrendStudenta.Clear();

                    foreach (var t in trend)
                        TrendStudenta.Add(t);
                });

                var poRokovima = await _ispitService.BrojIspitaPoIspitimRokovimaSviStudenti();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    IspitniRokovi.Clear();
                    BrojIspitaPoRokovima.Clear();

                    foreach (var item in poRokovima)
                    {
                        IspitniRokovi.Add(item.Key);
                        BrojIspitaPoRokovima.Add(item.Value);
                    }
                });

                var distribucija = await _ispitService.BrojOcenaDistribucija();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Ocene.Clear();
                    BrojOcena.Clear();

                    foreach (var item in distribucija)
                    {
                        Ocene.Add(item.Key);
                        BrojOcena.Add(item.Value);
                    }
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju statistike: {ex.Message}");
            }
        }
    }
}