using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FrodLib.Utils
{
    public class PropertyChangedManager<TSource> where TSource : INotifyPropertyChanged
    {
        private List<Subscription> subscriptions = new List<Subscription>();
        private TSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeManager&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="source">The property changed source.</param>
        public PropertyChangedManager(TSource source)
        {
            this.source = source;
        }

        /// <summary>
        /// Registers a regular event handler for change notification.
        /// </summary>
        public IDisposable AddHandler(PropertyChangedEventHandler handler)
        {
            return AddSubscription(new Subscription
            {
                IsStatic = handler.Target == null,
                SubscriberReference = new WeakReference(handler.Target),
                MethodCallback = handler.GetMethodInfo()
            });
        }

        /// <summary>
        /// Unregisters the given event handler from change notification.
        /// </summary>
        /// <param name="handler">The value.</param>
        public void RemoveHandler(PropertyChangedEventHandler handler)
        {
            CleanupSubscribers();

            subscriptions.RemoveAll(s => s.SubscriberReference.Target == handler.Target && s.MethodCallback == handler.GetMethodInfo());
        }

        /// <summary>
        /// Notifies subscribers that the given property has changed.
        /// </summary>
        /// <param name="propertyExpression">A lambda expression that accesses a property, such as <c>x => x.Name</c> 
        /// (where the type of x is <typeparamref name="TSource"/>).</param>
        public void NotifyChanged(string propertyName)
        {
            if (subscriptions.Count == 0) return;
            CleanupSubscribers();

            //foreach (var subscription in subscriptions.Where(s => s.PropertyName == propertyName))
            //{
            //    subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { this.source });
            //}

            // Call "old-style" handlers with the right signature.
            foreach (var subscription in subscriptions.ToArray())
            {
                subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { this.source, new PropertyChangedEventArgs(propertyName) });
            }
        }

        private IDisposable AddSubscription(Subscription subscription)
        {
            CleanupSubscribers();

            subscriptions.Add(subscription);

            return new SubscriptionReference(this.subscriptions, subscription);
        }

        private void CleanupSubscribers()
        {
            subscriptions.RemoveAll(s => !s.IsStatic && !s.SubscriberReference.IsAlive);
        }

        public void Clear()
        {
            subscriptions.Clear();
        }

        /// <summary>
        /// Provides deterministic removal of a subscription without having to 
        /// create a separate class to hold the delegate reference. 
        /// Callers can simply keep the returned disposable from Subscribe 
        /// and use it to unsubscribe.
        /// </summary>
        private sealed class SubscriptionReference : IDisposable
        {
            private List<Subscription> subscriptions;
            private Subscription entry;

            public SubscriptionReference(List<Subscription> subscriptions, Subscription entry)
            {
                this.subscriptions = subscriptions;
                this.entry = entry;
            }

            public void Dispose()
            {
                this.subscriptions.Remove(this.entry);
            }
        }

        private class Subscription
        {
            public bool IsStatic { get; set; }
            public string PropertyName { get; set; }
            public WeakReference SubscriberReference { get; set; }
            public MethodInfo MethodCallback { get; set; }
        }
    }
}
