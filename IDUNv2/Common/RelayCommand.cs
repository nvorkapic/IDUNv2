using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDUNv2.Common
{
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _execute;

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
