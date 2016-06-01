using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDUNv2.Common
{
    public class ActionCommand<T> : ICommand
    {
        private Action<T> _execute;

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public ActionCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object param)
        {
            return true;
        }

        public void Execute(object param)
        {
            _execute((T)param);
        }
    }
}
