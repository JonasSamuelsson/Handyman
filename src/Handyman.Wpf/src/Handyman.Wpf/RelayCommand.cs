using System;
using System.Diagnostics;

namespace Handyman.Wpf
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(_ => execute(), canExecute == null ? default(Predicate<object>) : _ => canExecute())
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            : base(execute, canExecute)
        {
        }

        public RelayCommand(Action execute, IReadOnlyObservable<bool> observable)
            : base(_ => execute(), observable)
        {
        }
    }

    public class RelayCommand<T> : System.Windows.Input.ICommand
    {
        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
            CommandManager = WpfCommandManager.Instance;
        }

        public RelayCommand(Action<T> execute, IReadOnlyObservable<bool> observable)
            : this(execute, _ => observable.Value)
        {
            observable.PropertyChanged += delegate { CommandManager.InvalidateRequerySuggested(); };
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public ICommandManager CommandManager { get; set; }

        private class WpfCommandManager : ICommandManager
        {
            private WpfCommandManager() { }

            static WpfCommandManager()
            {
                Instance = new WpfCommandManager();
            }

            public static readonly WpfCommandManager Instance;

            public event EventHandler RequerySuggested
            {
                add { System.Windows.Input.CommandManager.RequerySuggested += value; }
                remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
            }

            public void InvalidateRequerySuggested()
            {
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}