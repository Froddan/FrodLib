using FrodLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !USE_FROD_READ_WRITE_LOCK_IMPLEMENTATION
using FrodReaderWriterLock = System.Threading.ReaderWriterLockSlim;
using LockRecursionPolicy = System.Threading.LockRecursionPolicy;
#endif

namespace FrodLib.Collections.Concurrent
{
    internal class ConcurrentEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        private readonly FrodReaderWriterLock _lock;

        public ConcurrentEnumerator(IEnumerable<T> inner, FrodReaderWriterLock @lock)
        {
            this._lock = @lock;
            this._lock.EnterReadLock();
            this._inner = inner.GetEnumerator();
        }

        T IEnumerator<T>.Current
        {
            get { return _inner.Current; }
        }

        void IDisposable.Dispose()
        {
            this._lock.ExitReadLock();
        }

        object IEnumerator.Current
        {
            get { return _inner.Current; }
        }

        bool IEnumerator.MoveNext()
        {
            return _inner.MoveNext();
        }

        void IEnumerator.Reset()
        {
            _inner.Reset();
        }
    }
}
