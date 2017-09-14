using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrodLib.Commands.WPF
{
    public class RelayCommand : RelayCommand<object>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        public RelayCommand(Action<object> execute)
            : base(execute, null, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        public RelayCommand(Action<object> execute, IInvoker invoker)
            : base(execute, null, invoker)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute, IInvoker invoker)
            : base(execute, canExecute, invoker)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        public RelayCommand(Action execute)
            : base((obj) => execute(), null, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
            : base((obj) => execute(), (obj) =>
            {
                if (canExecute != null)
                {
                    return canExecute();
                }
                else
                {
                    return true;
                }
            }, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        public RelayCommand(Action execute, IInvoker invoker)
            : base((obj) => execute(), null, invoker)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute, IInvoker invoker)
            : base((obj) => execute(), (obj) =>
            {
                if (canExecute != null)
                {
                    return canExecute();
                }
                else
                {
                    return true;
                }
            }, invoker)
        {

        }

        #endregion
    }

    public class RelayCommand<T> : FrodLib.Commands.RelayCommand<T>
    {
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute)
            : base(execute, null, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
             : base(execute, canExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute, IInvoker invoker)
            : base(execute, null, invoker)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute, IInvoker invoker) : base(execute, canExecute, invoker)
        {
        }

        #endregion

        public override void Execute(object parameter)
        {
            var execute = _execute;
            if (execute != null) execute((T)parameter);
            if (m_invoker != null) m_invoker.BeginInvoke(CommandManager.InvalidateRequerySuggested);
        }

        protected override void OnAddCanExecuteChangedHandler(EventHandler handler)
        {
            if (_hookupCanExecute)
            {
                CommandManager.RequerySuggested += handler;
                _controlEvent.Add(new WeakReference(handler));
            }
        }

        protected override void OnRemoveCanExecuteChangedHandler(EventHandler handler)
        {
            if (_hookupCanExecute)
            {
                CommandManager.RequerySuggested -= handler;
                _controlEvent.Remove(_controlEvent.Find(r => ((EventHandler)r.Target) == handler));
            }
        }
    }
}
