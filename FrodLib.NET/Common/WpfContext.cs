using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FrodLib
{
    class WpfContext : IInvoker
    {
        private readonly Dispatcher _dispatcher;

        public bool IsSynchronized
        {
            get
            {
                return this._dispatcher.Thread == Thread.CurrentThread;
            }
        }

        public WpfContext()
            : this(Dispatcher.CurrentDispatcher)
        {
        }

        public WpfContext(Dispatcher dispatcher)
        {
            Debug.Assert(dispatcher != null);

            this._dispatcher = dispatcher;
        }


        internal static bool IsInDesignMode
        {
            get
            {
                DependencyObject dep = new DependencyObject();
                return DesignerProperties.GetIsInDesignMode(dep);
            }
        }

        public void BeginInvoke(Action action)
        {
            throw new NotImplementedException();
        }

        public void BeginInvoke<T>(Action<T> action, T arg)
        {
            throw new NotImplementedException();
        }

        public void Invoke(Action action)
        {
            throw new NotImplementedException();
        }

        public T Invoke<T>(Func<T> func)
        {
            throw new NotImplementedException();
        }

        public Task BeginInvokeAsync(Action action)
        {
            throw new NotImplementedException();
        }

        public Task BeginInvokeAsync<T>(Action<T> action, T arg)
        {
            throw new NotImplementedException();
        }

        public Task InvokeAsync(Action action)
        {
            throw new NotImplementedException();
        }

        public Task<T> InvokeAsync<T>(Func<T> func)
        {
            throw new NotImplementedException();
        }
    }
}
