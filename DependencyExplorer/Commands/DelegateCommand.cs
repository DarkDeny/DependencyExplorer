using System;
using System.Windows.Input;

namespace DependencyExplorer.Commands {
    public class DelegateCommand : ICommand {
        private readonly Predicate<object> _CanExecute;
        private readonly Action _Execute;

        public DelegateCommand(Action execute, Predicate<object> canExecute = null) {
            _CanExecute = canExecute;
            _Execute = execute;
        }

        public bool CanExecute(object parameter) {
            return _CanExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter) {
            _Execute();
        }

        public void FireCanExecuteChanged() {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;
    }
}
