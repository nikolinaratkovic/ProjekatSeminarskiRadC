using Projekat.ViewModels;
using ScottPlot;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Projekat.Views
{
    public partial class StatistikaView : UserControl
    {
        public StatistikaView()
        {
            InitializeComponent();

            if (DataContext is StatistikaViewModel vm)
            {
                vm.ProsecneOcenePredmeta.CollectionChanged += (s, e) => RefreshBarChart();
                vm.NaziviPredmeta.CollectionChanged += (s, e) => RefreshBarChart();
                vm.TrendStudenta.CollectionChanged += (s, e) => RefreshLineChart();
                vm.BrojIspitaPoRokovima.CollectionChanged += (s, e) => RefreshPieChart();
                vm.IspitniRokovi.CollectionChanged += (s, e) => RefreshPieChart();
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Projekat.ViewModels.StatistikaViewModel vm)
            {
                await vm.UcitajStatistikuAsync();
            }
        }

        private void RefreshBarChart()
        {
            if (DataContext is StatistikaViewModel vm && vm.ProsecneOcenePredmeta.Any())
            {
                var plt = BarChart.Plot;
                plt.Clear();

                var values = vm.ProsecneOcenePredmeta.ToArray();
                var labels = vm.NaziviPredmeta.ToArray();

                var bar = plt.AddBar(values);
                bar.FillColor = System.Drawing.Color.CornflowerBlue;
                bar.Label = "Prosečne ocene";

                plt.XTicks(labels);
                plt.Title("Prosečne ocene po predmetima");
                plt.YLabel("Ocena");

                BarChart.Refresh();
            }
        }

        private void RefreshLineChart()
        {
            if (DataContext is StatistikaViewModel vm && vm.TrendStudenta.Any())
            {
                var plt = LineChart.Plot;
                plt.Clear();

                var xs = Enumerable.Range(1, vm.TrendStudenta.Count).Select(i => (double)i).ToArray();
                var ys = vm.TrendStudenta.ToArray();

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

        private void RefreshPieChart()
        {
            if (DataContext is StatistikaViewModel vm && vm.BrojIspitaPoRokovima.Any())
            {
                var plt = PieChart.Plot;
                plt.Clear();

                var values = vm.BrojIspitaPoRokovima.Select(v => (double)v).ToArray();
                var labels = vm.IspitniRokovi.ToArray();

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