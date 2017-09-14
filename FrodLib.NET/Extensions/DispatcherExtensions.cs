using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace FrodLib.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DispatcherExtensions
    {

        /// <summary>
        /// Invokes the action async on the main thread.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        public static void InvokeAsync(this Dispatcher dispatcher, Action action)
        {
            if(dispatcher == null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                {
                    action();
                }));
            }
        }


        /// <summary>
        /// Invokes the action async on the main thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="param">parameters for the action.</param>
        public static void InvokeAsync<T>(this Dispatcher dispatcher, Action<T> action, T param)
        {
            if (dispatcher == null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            if (dispatcher.CheckAccess())
            {
                action(param);
            }
            else
            {
                dispatcher.BeginInvoke(new Action(() =>
                {
                    action(param);
                }));
            }
        }

        /// <summary>
        /// Invokes the action on the main thread, and waits for it to return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        public static void InvokeAndWait(this Dispatcher dispatcher, Action action)
        {
            if(dispatcher == null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }

        /// <summary>
        /// Invokes the action on the main thread, and waits for it to return
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="param">parameters for the action.</param>
        public static void InvokeAndWait<T>(this Dispatcher dispatcher, Action<T> action, T param)
        {
            if (dispatcher == null)
            {
                dispatcher = Dispatcher.CurrentDispatcher;
            }
            if (dispatcher.CheckAccess())
            {
                action(param);
            }
            else
            {
                dispatcher.Invoke(new Action(() =>
                {
                    action(param);
                }));

            }
        }
    }
}
