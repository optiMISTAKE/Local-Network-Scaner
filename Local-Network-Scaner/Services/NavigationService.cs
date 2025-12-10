using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Local_Network_Scanner.ViewModel.Base;

namespace Local_Network_Scanner.Services
{
    public class NavigationService
    {
        private Action<ViewModelBase> _navigate;

        public void SetNavigator(Action<ViewModelBase> navigator)
        {
            _navigate = navigator;
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            _navigate?.Invoke(viewModel);
        }
    }
}
