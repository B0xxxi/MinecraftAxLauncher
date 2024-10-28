// Utilities/AsyncRelayCommand.cs

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AxLauncher.Utilities
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> execute;
        private readonly Predicate<object> canExecute;

        public AsyncRelayCommand(Func<object, Task> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        private async Task ExecuteAsync(object parameter)
        {
            await execute(parameter);
        }
    }
}
