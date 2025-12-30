using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Local_Network_Scanner.ViewModel.Base;
using Local_Network_Scanner.Services;
using Local_Network_Scanner.Model;
using System.Windows.Input;

namespace Local_Network_Scanner.ViewModel
{
    public class MainMenuViewModel: ViewModelBase
    {
        // PRIVATE FIELDS
        private readonly NavigationService _navigationService;
        private readonly ViewModelFactory _viewModelFactory;

        // PUBLIC PROPERTIES, AVAILABLE FOR DATA BINDING

        // COMMANDS
        public ICommand NavigateToMainScanCommand { get; }
        public ICommand NavigateToBluetoothScanningCommand { get; }

        // CONSTRUCTORS
        public MainMenuViewModel(NavigationService navigationService, ViewModelFactory viewModelFactory)
        {
            _navigationService = navigationService;
            _viewModelFactory = viewModelFactory;

            NavigateToMainScanCommand = new RelayCommand(_ =>
            _navigationService.NavigateTo(_viewModelFactory.CreateMainScanVM()), _ => true);

            NavigateToBluetoothScanningCommand = new RelayCommand(_ =>
            _navigationService.NavigateTo(_viewModelFactory.CreateBluetoothScanVM()), _ => true);
        }
    }
}
