using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public MainViewModel()
        {
            //ShowStudentsCommand = new RelayCommand(_ => CurrentViewModel = new StudentsViewModel());
            //ShowPredmetiCommand = new RelayCommand(_ => CurrentViewModel = new PredmetiViewModel());

            //CurrentViewModel = new StudentsViewModel(); // default
        }
    }   
}
