using System;
using System.Collections.Generic;
using FrodLib.Extensions;

namespace FrodLib.Utils
{
    public abstract class Observable<T> : IObservable<T>, IDisposable
    {
        private readonly object lockObject = new object();
        private bool m_isDisposed;
        private IList<IObserver<T>> m_subscribers;

        public Observable()
        {
            m_subscribers = new List<IObserver<T>>();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            
            if (m_isDisposed)
            {
                throw new ObjectDisposedException("Observable<T>");
            }

            ArgumentValidator.IsNotNull(observer, nameof(observer));

            lock (lockObject)
            {
                m_subscribers.Add(observer);
            }

            return new Unsubscriber(() =>
            {
                lock (lockObject)
                {
                    m_subscribers.Remove(observer);
                }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (lockObject)
            {
                if (disposing)
                {
                    OnCompleted();
                    m_subscribers.Clear();
                    m_isDisposed = true;
                }
            }
        }

        protected void OnCompleted()
        {
            lock (lockObject)
            {
                if (m_isDisposed)
                {
                    throw new ObjectDisposedException("Observable<T>");
                }

                foreach (IObserver<T> observer in m_subscribers)
                {
                    observer.OnCompleted();
                }
            }
        }

        protected void OnError(Exception exception)
        {
            lock (lockObject)
            {
                if (m_isDisposed)
                {
                    throw new ObjectDisposedException("Observable<T>");
                }

                ArgumentValidator.IsNotNull(exception, nameof(exception));

                foreach (IObserver<T> observer in m_subscribers)
                {
                    observer.OnError(exception);
                }
            }
        }

        protected void OnNext(T value)
        {
            lock (lockObject)
            {
                if (m_isDisposed)
                {
                    throw new ObjectDisposedException("Observable<T>");
                }

                foreach (IObserver<T> observer in m_subscribers)
                {
                    observer.OnNext(value);
                }
            }
        }

        private class Unsubscriber : IDisposable
        {
            private Action m_dispose;

            internal Unsubscriber(Action dispose)
            {
                m_dispose = dispose;
            }

            public void Dispose()
            {
                m_dispose();
            }
        }
    }
}