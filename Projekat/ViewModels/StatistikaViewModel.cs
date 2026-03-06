using Projekat.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        public StatistikaViewModel(IspitService ispitService, PredmetService predmetService, StudentService studentService)
        {
            _ispitService = ispitService;
            _predmetService = predmetService;
            _studentService = studentService;

            Task.Run(async () => await UcitajStatistikuAsync());
        }

        private async Task UcitajStatistikuAsync()
        {
            try
            {
                var predmeti = await _predmetService.ucitajSvePredmete();
                foreach (var predmet in predmeti)
                {
                    double prosek = await _ispitService.izracunajProsecnuOcenuPredmeta(predmet.PredmetID);
                    ProsecneOcenePredmeta.Add(prosek);
                    NaziviPredmeta.Add(predmet.Naziv);
                }

                var studenti = await _studentService.ucitajSveStudente();
                foreach (var student in studenti)
                {
                    double prosekStudenta = await _ispitService.izracunajProsecnuOcenuStudenta(student.StudentID);
                    TrendStudenta.Add(prosekStudenta);
                }

                var poRokovima = await _ispitService.brojIspitaPoIspitimRokovimaSviStudenti();
                foreach (var item in poRokovima)
                {
                    IspitniRokovi.Add(item.Key);
                    BrojIspitaPoRokovima.Add(item.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri učitavanju statistike: {ex.Message}");
            }
        }
    }
}