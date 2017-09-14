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
    public class ConcurrentList<T> : IList<T>
    {
        private FrodReaderWriterLock m_lock;
        private IList<T> m_list;

        public ConcurrentList()
        {
            this.m_lock = new FrodReaderWriterLock(LockRecursionPolicy.NoRecursion);
            this.m_list = new List<T>();
        }

        public ConcurrentList(int capacity)
        {
            this.m_lock = new FrodReaderWriterLock(LockRecursionPolicy.NoRecursion);
            this.m_list = new List<T>(capacity);
        }

        public ConcurrentList(IEnumerable<T> items)
        {
            this.m_lock = new FrodReaderWriterLock(LockRecursionPolicy.NoRecursion);
            this.m_list = new List<T>(items);
        }

        public T this[int index]
        {
            get
            {
                try
                {
                    m_lock.EnterReadLock();
                    return m_list[index];
                }
                finally
                {
                    m_lock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    m_lock.EnterWriteLock();
                    m_list[index] = value;
                }
                finally
                {
                    m_lock.ExitWriteLock();
                }
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    m_lock.EnterReadLock();
                    return m_list.Count;
                }
                finally
                {
                    m_lock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                try
                {
                    m_lock.EnterReadLock();
                    return m_list.IsReadOnly;
                }
                finally
                {
                    m_lock.ExitReadLock();
                }
            }
        }

        public void Add(T item)
        {
            try
            {
                m_lock.EnterWriteLock();
                m_list.Add(item);
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            try
            {
                m_lock.EnterWriteLock();
                m_list.Clear();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            try
            {
                m_lock.EnterReadLock();
                return m_list.Contains(item);
            }
            finally
            {
                m_lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                m_lock.EnterReadLock();
                m_list.CopyTo(array, arrayIndex);
            }
            finally
            {
                m_lock.ExitReadLock();
            }
        }

        public int IndexOf(T item)
        {
            try
            {
                m_lock.EnterReadLock();
                return m_list.IndexOf(item);
            }
            finally
            {
                m_lock.ExitReadLock();
            }
        }

        public void Insert(int index, T item)
        {
            try
            {
                m_lock.EnterWriteLock();
                m_list.Clear();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public bool Remove(T item)
        {
            try
            {
                m_lock.EnterWriteLock();
                return m_list.Remove(item);
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            try
            {
                m_lock.EnterWriteLock();
                m_list.RemoveAt(index);
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ConcurrentEnumerator<T>(this.m_list, this.m_lock);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ConcurrentEnumerator<T>(this.m_list, this.m_lock);
        }

        ~ConcurrentList()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

            this.m_lock.Dispose();
        }
    }
}
