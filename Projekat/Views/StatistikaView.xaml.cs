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
            Loaded += StatistikaView_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is StatistikaViewModel vm)
            {
                await vm.UcitajStatistikuAsync();

                RefreshBarChart();
                RefreshLineChart();
                RefreshPieChart();
                RefreshDistributionChart();
            }
        }

        private void StatistikaView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is StatistikaViewModel vm)
            {
                vm.ProsecneOcenePredmeta.CollectionChanged += (s, e) => RefreshBarChart();
                vm.NaziviPredmeta.CollectionChanged += (s, e) => RefreshBarChart();

                vm.TrendStudenta.CollectionChanged += (s, e) => RefreshLineChart();

                vm.IspitniRokovi.CollectionChanged += (s, e) => RefreshPieChart();
                vm.BrojIspitaPoRokovima.CollectionChanged += (s, e) => RefreshPieChart();

                vm.Ocene.CollectionChanged += (s, e) => RefreshDistributionChart();
                vm.BrojOcena.CollectionChanged += (s, e) => RefreshDistributionChart();
            }
        }

        private void RefreshBarChart()
        {
            if (DataContext is not StatistikaViewModel vm)
                return;

            if (!vm.ProsecneOcenePredmeta.Any() || !vm.NaziviPredmeta.Any())
                return;

            var plt = BarChart.Plot;
            plt.Clear();

            var values = vm.ProsecneOcenePredmeta.ToArray();

            var labels = vm.NaziviPredmeta
                .Select(n => n.Length > 10 ? n.Substring(0, 10) + "..." : n)
                .ToArray();

            var bar = plt.AddBar(values);
            bar.FillColor = System.Drawing.Color.CornflowerBlue;

            plt.XTicks(labels);
            plt.XAxis.TickLabelStyle(rotation: 60);
            plt.XAxis.TickLabelStyle(fontSize: 10);
            plt.Layout(bottom: 120);

            plt.Grid(enable: true);
            plt.Title("Prosečne ocene po predmetima");
            plt.YLabel("Ocena");

            BarChart.Refresh();
        }

        private void RefreshLineChart()
        {
            if (DataContext is not StatistikaViewModel vm)
                return;

            if (!vm.TrendStudenta.Any())
                return;

            var plt = LineChart.Plot;
            plt.Clear();

            var xs = Enumerable.Range(0, vm.TrendStudenta.Count)
                               .Select(i => (double)i)
                               .ToArray();

            var ys = vm.TrendStudenta.ToArray();

            var line = plt.AddScatter(xs, ys);
            line.LineWidth = 2;
            line.Color = System.Drawing.Color.Orange;
            line.MarkerSize = 5;
            plt.Grid(enable: true);
            plt.Title("Trend studenata");
            plt.YLabel("Prosek ocena");
            LineChart.Refresh();
        }

        private void RefreshPieChart()
        {
            if (DataContext is not StatistikaViewModel vm)
                return;

            if (vm.BrojIspitaPoRokovima.Count == 0 || vm.IspitniRokovi.Count == 0)
                return;

            var plt = PieChart.Plot;
            plt.Clear();

            double[] values = vm.BrojIspitaPoRokovima.Select(x => (double)x).ToArray();
            string[] labels = vm.IspitniRokovi.ToArray();

            if (values.Length == 0)
                return;

            var pie = plt.AddPie(values);
            pie.SliceLabels = labels;
            pie.ShowLabels = false;
            pie.Explode = true;
            plt.Title("Ispiti po rokovima");
            plt.Legend(location: ScottPlot.Alignment.LowerRight);
            plt.Layout(top: 60, right: 180);
            PieChart.Refresh();
        }

        private void RefreshDistributionChart()
        {
            if (DataContext is not StatistikaViewModel vm)
                return;

            if (vm.Ocene.Count == 0 || vm.BrojOcena.Count == 0)
                return;

            var plt = DistributionChart.Plot;
            plt.Clear();

            var values = vm.BrojOcena.Select(x => (double)x).ToArray();
            var labels = vm.Ocene.Select(x => x.ToString()).ToArray();

            var bar = plt.AddBar(values);
            bar.PositionOffset = 0;
            plt.XTicks(labels);
            plt.Title("Distribucija ocena");
            plt.YLabel("Broj ispita");
            DistributionChart.Refresh();
        }
    }
}