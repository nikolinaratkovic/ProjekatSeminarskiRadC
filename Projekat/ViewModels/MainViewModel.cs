using Projekat.Data;
using Projekat.Services;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace Projekat.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowStudentsCommand { get; }
        public ICommand ShowPredmetiCommand { get; }
        public ICommand ShowIspitiCommand { get; }
        public ICommand ShowStatistikaCommand { get; }

        private readonly IspitService _ispitService;
        private readonly PredmetService _predmetService;
        private readonly StudentService _studentService;

        public MainViewModel()
        {
           var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(Config.ConnectionString)
                .Options;

            var context = new ApplicationDbContext(options);

            _ispitService = new IspitService(context);
            _predmetService = new PredmetService(context);
            _studentService = new StudentService(context);

            CurrentViewModel = new StudentViewModel();

            ShowStudentsCommand = new RelayCommand(_ => CurrentViewModel = new StudentViewModel());
            ShowPredmetiCommand = new RelayCommand(_ => CurrentViewModel = new PredmetViewModel());
            ShowIspitiCommand = new RelayCommand(_ => CurrentViewModel = new IspitViewModel());
            ShowStatistikaCommand = new RelayCommand(_ => CurrentViewModel = new StatistikaViewModel(_ispitService, _predmetService, _studentService));
        }
    }
}