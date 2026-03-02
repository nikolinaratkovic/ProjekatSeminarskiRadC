using System.Windows.Input;

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
        public ICommand ShowIspititCommand { get; }

        public MainViewModel()
        {
            CurrentViewModel = new StudentViewModel(); // default view

            ShowStudentsCommand = new RelayCommand(_ => CurrentViewModel = new StudentViewModel());
            ShowPredmetiCommand = new RelayCommand(_ => CurrentViewModel = new PredmetViewModel());
            ShowIspititCommand = new RelayCommand(_ => CurrentViewModel = new IspitViewModel());
        }
    }
}