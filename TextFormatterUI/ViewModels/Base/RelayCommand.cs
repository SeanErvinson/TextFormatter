using System;
using System.Windows.Input;

namespace TextFormatterUI
{
    class RelayCommand : ICommand
    {
        private readonly Predicate<object> _verifyAction;
        private readonly Action<object> _methodAction;
        
        public RelayCommand(Action<object> execute)
        {
            _methodAction = execute ?? throw new ArgumentException("execute");
        }

        public RelayCommand(Action<object> execute, Predicate<object> validation)
            : this(execute)
        {
            _verifyAction = validation;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _verifyAction == null ? true : _verifyAction(parameter);
        }

        public void Execute(object parameter)
        {
            _methodAction(parameter);
        }
    }
}
