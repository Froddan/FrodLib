using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using System.Windows.Input;
using FrodLib.Extensions;


namespace FrodLib.Commands
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

    public class RelayCommand<T> : ICommand, IDisposable
    {
        #region Fields

        protected readonly bool _hookupCanExecute;
        protected Action<T> _execute = null;
        private Predicate<T> _canExecute = null;
        private List<INotifyPropertyChanged> propertiesToListenTo;
        protected readonly List<WeakReference> _controlEvent;
        private List<INotifyPropertyChanged> m_listensOn = new List<INotifyPropertyChanged>();
        protected readonly IInvoker m_invoker;


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute)
            : this(execute, null, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
             : this(execute, canExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute, IInvoker invoker)
            : this(execute, null, invoker)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute, IInvoker invoker)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _controlEvent = new List<WeakReference>();
            _execute = execute;
            _canExecute = canExecute;
            _hookupCanExecute = canExecute != null;
            m_invoker = invoker;
        }

        #endregion

        public List<INotifyPropertyChanged> PropertiesToListenTo
        {
            get { return propertiesToListenTo; }
            set
            {
                propertiesToListenTo = value;
            }
        }

        #region ICommand Members

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        ///<returns>
        ///true if this command can be executed; otherwise, false.
        ///</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        ///<summary>
        ///Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                OnAddCanExecuteChangedHandler(value);
            }
            remove
            {
                OnRemoveCanExecuteChangedHandler(value);
            }
        }

        protected virtual void OnAddCanExecuteChangedHandler(EventHandler handler)
        {
            if (_hookupCanExecute)
            {
                _controlEvent.Add(new WeakReference(handler));
            }
        }

        protected virtual void OnRemoveCanExecuteChangedHandler(EventHandler handler)
        {
            if (_hookupCanExecute)
            {
                _controlEvent.Remove(_controlEvent.Find(r => ((EventHandler)r.Target) == handler));
            }
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public virtual void Execute(object parameter)
        {
            var execute = _execute;
            if (execute != null) execute((T)parameter);
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            if (_controlEvent != null && _controlEvent.Count > 0)
            {
                if (m_invoker != null)
                {
                    m_invoker.BeginInvoke(new Action(() =>
                  {
                      _controlEvent.ForEach(ce =>
                      {
                          if (ce.Target != null)
                          {
                              ((EventHandler)(ce.Target)).Invoke(null, EventArgs.Empty);
                          }
                      });
                  }));
                }
            }
        }

        //public RelayCommand<T> ListenOn<TObservedType, TPropertyType>(TObservedType viewModel, Expression<Func<TObservedType, TPropertyType>> propertyExpression) where TObservedType : INotifyPropertyChanged
        //{
        //    if (viewModel == null) return this;
        //    string propertyName = GetPropertyName(propertyExpression);
        //    viewModel.PropertyChanged += (PropertyChangedEventHandler)((sender, e) =>
        //    {
        //        if (e.PropertyName == propertyName) RaiseCanExecuteChanged();
        //    });
        //    return this;
        //}

        public RelayCommand<T> ListenForNotificationFrom<TObservedType>(TObservedType viewModel) where TObservedType : INotifyPropertyChanged
        {
            if (viewModel == null) return this;
            if (!m_listensOn.Contains(viewModel))
            {
                viewModel.PropertyChanged += ViewModelPropertyChanged;
                m_listensOn.Add(viewModel);
            }
            return this;
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        private string GetPropertyName<K, TProperty>(Expression<Func<K, TProperty>> expression) where K : INotifyPropertyChanged
        {
            var lambda = expression as LambdaExpression;
            MemberInfo memberInfo = GetmemberExpression(lambda).Member;
            return memberInfo.Name;
        }

        private MemberExpression GetmemberExpression(LambdaExpression lambda)
        {
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
                memberExpression = lambda.Body as MemberExpression;
            return memberExpression;
        }

        public void Dispose()
        {
            foreach (var item in m_listensOn)
            {
                item.PropertyChanged -= ViewModelPropertyChanged;
            }
            m_listensOn.Clear();
            _controlEvent.Clear();
            _execute = null;
            _canExecute = null;
        }

    }
}

