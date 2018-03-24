using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TextFormatter.WPF.ViewModels.Base
{
    class AsyncRelayCommand : ICommand
    {
        /// <summary>
        /// Delegate to call when CanExecute method is called.
        /// </summary>
        protected readonly Predicate<object> canExecute;

        /// <summary>
        /// Delegate to call when Execute is called.
        /// </summary>
        protected Func<object, Task> asyncExecute;

        /// <summary>
        /// Event which is raised when the state of this command has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new async delegate command.
        /// </summary>
        /// <param name="execute">Method to call when command is executed.</param>
        public AsyncRelayCommand(Func<Task> execute)
            : this(_ => execute(), null)
        {
        }

        /// <summary>
        /// Creates a new async delegate command.
        /// </summary>
        /// <param name="execute">Method to call when command is executed.</param>
        public AsyncRelayCommand(Func<object, Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new async delegate command.
        /// </summary>
        /// <param name="execute">Method to call when command is executed.</param>
        /// <param name="canExecute">Method to call to determine whether command is valid.</param>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
            : this(_ => execute(), _ => canExecute())
        {
        }

        /// <summary>
        /// Creates a new async delegate command.
        /// </summary>
        /// <param name="asyncExecute">Method to call when command is executed.</param>
        /// <param name="canExecute">Method to call to determine whether command is valid.</param>
        public AsyncRelayCommand(Func<object, Task> asyncExecute,
            Predicate<object> canExecute)
        {
            this.asyncExecute = asyncExecute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Raise the CanExecuteChanged handler.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns whether the command is possible right now.
        /// </summary>
        /// <returns><c>true</c>, if execute was caned, <c>false</c> otherwise.</returns>
        /// <param name="parameter">Parameter.</param>
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <summary>
        /// Executes the command and returns an awaitable task.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="parameter">Parameter.</param>
        public async Task ExecuteAsync(object parameter)
        {
            await asyncExecute(parameter);
        }
    }
}
