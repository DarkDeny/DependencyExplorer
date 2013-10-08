using System;
using System.Windows.Input;

namespace DependencyExplorer.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _CanExecute;
        private readonly Action _Execute;

        public DelegateCommand(Predicate<object> canExecute, Action execute)
        {
            _CanExecute = canExecute;
            _Execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _Execute();
        }

        public void FireCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
