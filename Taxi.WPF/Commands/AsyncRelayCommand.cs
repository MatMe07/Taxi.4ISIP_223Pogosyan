using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Taxi.WPF.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        private Action<object> execute;
        private readonly Func<object?, Task>? _executeAsync;
        private Func<object, bool> canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value;}
        }
        
        public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object, bool> canExecute = null)
        {
            this._executeAsync = executeAsync;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting &&( this.canExecute == null || this.canExecute(parameter));
        }

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try
            {
                await _executeAsync(parameter);

            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }

        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
