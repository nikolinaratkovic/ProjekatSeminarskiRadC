using Projekat.ViewModels;
using ScottPlot;
using System.Linq;
using System.Windows.Controls;

namespace Projekat.Views
{
    public partial class StatistikaView : UserControl
    {
        public StatistikaView()
        {
            InitializeComponent();

            BarChart.Loaded += BarChart_Loaded;
            LineChart.Loaded += LineChart_Loaded;
            PieChart.Loaded += PieChart_Loaded;
        }

        private void BarChart_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is StatistikaViewModel vm && vm.ProsecneOcenePredmeta.Any())
            {
                var plt = BarChart.Plot;

                double[] values = vm.ProsecneOcenePredmeta.ToArray();
                string[] labels = vm.NaziviPredmeta.ToArray();

                var bar = plt.AddBar(values);
                bar.FillColor = System.Drawing.Color.CornflowerBlue;
                bar.Label = "Prosečne ocene";

                plt.XTicks(labels);
                plt.Title("Prosečne ocene po predmetima");
                plt.YLabel("Ocena");

                BarChart.Refresh();
            }
        }

        private void LineChart_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is StatistikaViewModel vm && vm.TrendStudenta.Any())
            {
                var plt = LineChart.Plot;

                double[] xs = Enumerable.Range(1, vm.TrendStudenta.Count).Select(i => (double)i).ToArray();
                double[] ys = vm.TrendStudenta.ToArray();

                var scatter = plt.AddScatter(xs, ys);
                scatter.LineWidth = 2;
                scatter.Color = System.Drawing.Color.Orange;
                scatter.MarkerSize = 5;

                plt.Title("Trend studenta");
                plt.XLabel("Studenti");
                plt.YLabel("Prosek ocena");

                LineChart.Refresh();
            }
        }

        private void PieChart_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is StatistikaViewModel vm && vm.BrojIspitaPoRokovima.Any())
            {
                var plt = PieChart.Plot;

                double[] values = vm.BrojIspitaPoRokovima.Select(v => (double)v).ToArray();
                string[] labels = vm.IspitniRokovi.ToArray();

                var pie = plt.AddPie(values);
                pie.SliceLabels = labels;
                pie.ShowPercentages = true;
                pie.Explode = true;

                plt.Title("Broj ispita po ispitnim rokovima");

                PieChart.Refresh();
            }
        }
    }
}