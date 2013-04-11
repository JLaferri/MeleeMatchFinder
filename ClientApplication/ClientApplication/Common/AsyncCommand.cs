using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientApplication.Common
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;
        private readonly Action _onStart;
        private readonly Action _onCompletion;
        private readonly Action<Exception> _onError;

        public AsyncCommand(Func<bool> canExecute, Action execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public AsyncCommand(Func<bool> canExecute, Action execute, Action onStart, Action onCompletion)
            : this(canExecute, execute)
        {
            _onStart = onStart;
            _onCompletion = onCompletion;
        }

        public AsyncCommand(Func<bool> canExecute, Action execute, Action onStart, Action onCompletion, Action<Exception> onError)
            : this(canExecute, execute, onStart, onCompletion)
        {
            _onError = onError;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_onStart != null) _onStart();

            Task.Factory.StartNew(() => _execute()).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    if (_onError != null) _onError(task.Exception);
                    else throw task.Exception;
                }

                if (_onCompletion != null) _onCompletion();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        public event EventHandler CanExecuteChanged;
    }

    public class AsyncCommand<TIn> : ICommand
    {
        private readonly Func<TIn, bool> _canExecute;
        private readonly Action<TIn> _execute;
        private readonly Action<TIn> _onStart;
        private readonly Action<TIn> _onCompletion;
        private readonly Action<TIn, Exception> _onError;

        public AsyncCommand(Func<TIn, bool> canExecute, Action<TIn> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public AsyncCommand(Func<TIn, bool> canExecute, Action<TIn> execute, Action<TIn> onStart, Action<TIn> onCompletion)
            : this(canExecute, execute)
        {
            _onStart = onStart;
            _onCompletion = onCompletion;
        }

        public AsyncCommand(Func<TIn, bool> canExecute, Action<TIn> execute, Action<TIn> onStart, Action<TIn> onCompletion, Action<TIn, Exception> onError)
            : this(canExecute, execute, onStart, onCompletion)
        {
            _onError = onError;
        }

        public bool CanExecute(object parameter)
        {
            TIn input;
            try { input = (TIn)parameter; }
            catch { return false; }

            return _canExecute(input);
        }

        public void Execute(object parameter)
        {
            TIn input = (TIn)parameter;

            if (_onStart != null) _onStart(input);

            Task.Factory.StartNew(() => _execute(input)).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    if (_onError != null) _onError(input, task.Exception);
                    else throw task.Exception;
                }

                if (_onCompletion != null) _onCompletion(input);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        public event EventHandler CanExecuteChanged;
    }
}
