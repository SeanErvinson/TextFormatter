using System;
using System.Windows.Input;

namespace TextFormatter.WPF.ViewModels.Base
{
    class RelayCommand : ICommand
    {
        private readonly Func<object, bool> _verifyAction;
        private readonly Action<object> _methodAction;

        public RelayCommand(Action execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Func<object, bool> validation)
        {
            _methodAction = execute ?? throw new ArgumentNullException("_methodAction", "Command cannot be null.");
            _verifyAction = validation;
        }

        public RelayCommand(Action execute, Func<bool> validation)
        {
            if (execute == null)
                throw new ArgumentNullException("_methodAction", "Command cannot be null.");

            _methodAction = delegate {
                execute();
            };
            if (validation != null)
            {
                _verifyAction = delegate {
                    return validation();
                };
            }
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
