

using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FrodLib.Utils
{


    public class LockRecursionException : System.Threading.LockRecursionException
    {
        public LockRecursionException()
            : base()
        {
        }

        public LockRecursionException(string message)
            : base(message)
        {
        }

        public LockRecursionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

#if USE_FROD_READ_WRITE_LOCK_IMPLEMENTATION

    
    public enum LockRecursionPolicy
    {
        NoRecursion, SupportsRecursion
    }


    public sealed class FrodReaderWriterLock : IDisposable
    {
        private readonly object m_lockObject = new object();
        private LockHolder m_threadInUpgradeableReadMode;
        private LockHolder m_threadInWriteMode;
        private List<LockHolder> m_threadsInReadMode;
        private HashSet<Thread> m_threadsWaitingForReadLock;
        private HashSet<Thread> m_threadsWaitingForUpgradeableReadLock;
        private HashSet<Thread> m_threadsWaitingForWriteLock;

        public FrodReaderWriterLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public FrodReaderWriterLock(LockRecursionPolicy recursionPolicy)
        {
            RecursionPolicy = recursionPolicy;
            
            m_threadsInReadMode = new List<LockHolder>();
            m_threadsWaitingForWriteLock = new HashSet<Thread>();
            m_threadsWaitingForUpgradeableReadLock = new HashSet<Thread>();
            m_threadsWaitingForReadLock = new HashSet<Thread>();
        }

        /// <summary>
        /// Returns the number of threads that have read lock
        /// </summary>
        /// <remarks>
        /// A thread is counted only once, even if the lock allows recursion and the thread has entered read mode multiple times.
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int CurrentReadCount
        {
            get
            {
                lock (m_lockObject)
                {
                    int readCount = m_threadsInReadMode.Count;
                    if (!m_threadsInReadMode.Any(t => t.Thread == Thread.CurrentThread) && IsUpgradeableReadLockHeld)
                    {
                        readCount++;
                    }
                    return readCount;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current thread has entered the lock in read mode. 
        /// </summary>
        /// <remarks>This property is intended for use in asserts or for other debugging purposes. Do not use it to control the flow of program execution.</remarks>
        public bool IsReadLockHeld
        {
            get
            {
                lock (m_lockObject)
                {
                    return m_threadsInReadMode.Any(t => t.Thread == Thread.CurrentThread);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current thread has entered the lock in upgradeable mode. 
        /// </summary>
        /// <remarks>This property is intended for use in asserts or for other debugging purposes. Do not use it to control the flow of program execution.</remarks>
        public bool IsUpgradeableReadLockHeld
        {
            get
            {
                lock (m_lockObject)
                {
                    return IsCurrentThreadHoldingLock(m_threadInUpgradeableReadMode);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current thread has entered the lock in write mode. 
        /// </summary>
        /// <remarks>This property is intended for use in asserts or for other debugging purposes. Do not use it to control the flow of program execution.</remarks>
        public bool IsWriteLockHeld
        {
            get
            {
                lock (m_lockObject)
                {
                    return IsCurrentThreadHoldingLock(m_threadInWriteMode);
                }
            }
        }

        public LockRecursionPolicy RecursionPolicy { get; private set; }

        /// <summary>
        /// Gets the number of times the current thread has entered the lock in read mode, as an indication of recursion.
        /// </summary>
        /// <remarks>
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int RecursiveReadCount
        {
            get
            {
                lock (m_lockObject)
                {
                    LockHolder holder = m_threadsInReadMode.FirstOrDefault(t => t.Thread == Thread.CurrentThread);
                    if (holder != null)
                    {
                        return holder.NumberOfLocks;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of times the current thread has entered the lock in upgradeable mode, as an indication of recursion.
        /// </summary>
        /// <remarks>
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int RecursiveUpgradeableReadCount
        {
            get
            {
                lock (m_lockObject)
                {
                    LockHolder holder = m_threadInUpgradeableReadMode;
                    if (IsCurrentThreadHoldingLock(holder))
                    {
                        return holder.NumberOfLocks;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of times the current thread has entered the lock in write mode, as an indication of recursion.
        /// </summary>
        /// <remarks>
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int RecursiveWriteCount
        {
            get
            {
                lock (m_lockObject)
                {
                    LockHolder holder = m_threadInWriteMode;
                    if (IsCurrentThreadHoldingLock(holder))
                    {
                        return holder.NumberOfLocks;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the total number of threads that are waiting to enter the lock in read mode.
        /// </summary>
        /// <remarks>
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int WaitingReadCount
        {
            get
            {
                lock (m_lockObject)
                {
                    return m_threadsWaitingForReadLock.Count;
                }
            }
        }

        /// <summary>
        /// Gets the total number of threads that are waiting to enter the lock in upgradeable mode.
        /// </summary>
        /// <remarks>
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int WaitingUpgradeableReadCount
        {
            get
            {
                lock (m_lockObject)
                {
                    return m_threadsWaitingForUpgradeableReadLock.Count;
                }
            }
        }

        /// <summary>
        /// Gets the total number of threads that are waiting to enter the lock in write mode.
        /// </summary>
        /// <remarks>
        /// Use this property only for debugging, profiling, and logging purposes, 
        /// and not to control the behavior of an algorithm. The results can change as soon as they have been calculated. 
        /// Therefore, it is not safe to make decisions based on this property.
        /// </remarks>
        public int WaitingWriteCount
        {
            get
            {
                return m_threadsWaitingForWriteLock.Count;
            }
        }

        public void EnterReadLock()
        {
            Thread currentThread = Thread.CurrentThread;

            lock (m_lockObject)
            {
                if (m_threadsInReadMode.Any(t => t.Thread == currentThread) || IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }

                    LockHolder holder = m_threadsInReadMode.FirstOrDefault(t => t.Thread == currentThread);
                    if (holder == null)
                    {
                        holder = new LockHolder(currentThread);
                        m_threadsInReadMode.Add(holder);
                    }
                    else
                    {
                        holder.NumberOfLocks++;
                    }
                }
                else
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadsWaitingForReadLock.Add(currentThread);
                    while (m_threadInWriteMode != null || m_threadsWaitingForWriteLock.Any())
                    {
                        Monitor.Wait(m_lockObject);
                    }
                    m_threadsWaitingForReadLock.Remove(currentThread);
                    LockHolder holder = new LockHolder(currentThread);
                    m_threadsInReadMode.Add(holder);
                }
            }
        }

        public void EnterUpgradeableReadLock()
        {
            lock (m_lockObject)
            {
                Thread currentThread = Thread.CurrentThread;
                if (IsCurrentThreadHoldingLock(m_threadInUpgradeableReadMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadInUpgradeableReadMode.NumberOfLocks++;
                }
                else if (m_threadInUpgradeableReadMode == null && IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadInUpgradeableReadMode = new LockHolder(currentThread);
                }
                else
                {
                    if (m_threadsInReadMode.Any(t => t.Thread == currentThread))
                    {
                        throw new LockRecursionException(StringResources.CannotAcquireUpgradeableLockWhileReadLock);
                    }
                    else if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }

                    m_threadsWaitingForUpgradeableReadLock.Add(currentThread);
                    while (m_threadInUpgradeableReadMode != null || m_threadInWriteMode != null || m_threadsWaitingForWriteLock.Any())
                    {
                        Monitor.Wait(m_lockObject);
                    }
                    m_threadsWaitingForUpgradeableReadLock.Remove(currentThread);
                    m_threadInUpgradeableReadMode = new LockHolder(currentThread);
                }
            }
        }

        public void EnterWriteLock()
        {
            lock (m_lockObject)
            {
                Thread currentThread = Thread.CurrentThread;
                if (IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadInWriteMode.NumberOfLocks++;
                }
                else
                {
                    if (m_threadsInReadMode.Any(t => t.Thread == currentThread))
                    {
                        throw new LockRecursionException(StringResources.CannotAcquireWriteLockWhileReadLock);
                    }
                    else if (!IsCurrentThreadHoldingLock(m_threadInUpgradeableReadMode, currentThread) && RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }

                    m_threadsWaitingForWriteLock.Add(currentThread);
                    while (m_threadInWriteMode != null && m_threadInWriteMode.Thread != currentThread || m_threadsInReadMode.Any())
                    {
                        Monitor.Wait(m_lockObject);
                    }
                    m_threadsWaitingForWriteLock.Remove(currentThread);
                    m_threadInWriteMode = new LockHolder(currentThread);
                }
            }
        }

        public void Dispose()
        {
            lock (m_lockObject)
            {
                m_threadsWaitingForUpgradeableReadLock.Clear();
                m_threadsInReadMode.Clear();
                m_threadInWriteMode = null;
                m_threadInUpgradeableReadMode = null;
                m_threadsWaitingForWriteLock.Clear();
                Monitor.PulseAll(m_lockObject);
            }
        }

        public void ExitReadLock()
        {
            Thread currentThread = Thread.CurrentThread;
            lock (m_lockObject)
            {
                LockHolder holder = m_threadsInReadMode.FirstOrDefault(t => t.Thread == currentThread);
                if (holder != null)
                {
                    holder.NumberOfLocks--;
                    if (holder.NumberOfLocks == 0)
                    {
                        m_threadsInReadMode.Remove(holder);
                        Monitor.PulseAll(m_lockObject);
                    }
                }
                else
                {
                    throw new InvalidOperationException(StringResources.ThreadDoesntHoldReadLock);
                }
            }
        }

        public void ExitUpgradeableReadLock()
        {
            lock (m_lockObject)
            {
                Thread currentThread = Thread.CurrentThread;
                if (m_threadInUpgradeableReadMode == null || m_threadInUpgradeableReadMode.Thread != currentThread)
                {
                    throw new InvalidOperationException(StringResources.ThreadDoesntHoldUpgradeableLock);
                }
                else
                {
                    m_threadInUpgradeableReadMode.NumberOfLocks--;
                    if (m_threadInUpgradeableReadMode.NumberOfLocks == 0)
                    {
                        m_threadInUpgradeableReadMode = null;
                        Monitor.PulseAll(m_lockObject);
                    }
                }
            }
        }

        public void ExitWriteLock()
        {
            lock (m_lockObject)
            {
                Thread currentThread = Thread.CurrentThread;
                if (m_threadInWriteMode == null || m_threadInWriteMode.Thread != currentThread)
                {
                    throw new InvalidOperationException(StringResources.ThreadDoesntHoldWriteLock);
                }
                else
                {
                    m_threadInWriteMode.NumberOfLocks--;
                    if (m_threadInWriteMode.NumberOfLocks == 0)
                    {
                        m_threadInWriteMode = null;
                        Monitor.PulseAll(m_lockObject);
                    }
                }
            }
        }

        public bool TryEnterReadLock(TimeSpan timeout)
        {
            Thread currentThread = Thread.CurrentThread;
            lock (m_lockObject)
            {
                if (m_threadsInReadMode.Any(t => t.Thread == currentThread) || IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }

                    LockHolder holder = m_threadsInReadMode.FirstOrDefault(t => t.Thread == currentThread);
                    if (holder == null)
                    {
                        holder = new LockHolder(currentThread);
                        m_threadsInReadMode.Add(holder);
                    }
                    else
                    {
                        holder.NumberOfLocks++;
                    }
                }
                else
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadsWaitingForReadLock.Add(currentThread);
                    while (m_threadInWriteMode != null || m_threadsWaitingForWriteLock.Any())
                    {
                        if (!Monitor.Wait(m_lockObject, timeout))
                        {
                            return false;
                        }
                    }
                    m_threadsWaitingForReadLock.Remove(currentThread);
                    LockHolder holder = new LockHolder(currentThread);
                    m_threadsInReadMode.Add(holder);
                }
            }
            return true;
        }

        public bool TryEnterReadLock(int millisecondsTimeout)
        {
            return TryEnterReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
        {
            return TryEnterUpgradeableReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
        {
            lock (m_lockObject)
            {
                Thread currentThread = Thread.CurrentThread;
                if (IsCurrentThreadHoldingLock(m_threadInUpgradeableReadMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadInUpgradeableReadMode.NumberOfLocks++;
                }
                else if (m_threadInUpgradeableReadMode == null && IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadInUpgradeableReadMode = new LockHolder(currentThread);
                }
                else
                {
                    if (m_threadsInReadMode.Any(t => t.Thread == currentThread))
                    {
                        throw new LockRecursionException(StringResources.CannotAcquireUpgradeableLockWhileReadLock);
                    }
                    else if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }

                    m_threadsWaitingForUpgradeableReadLock.Add(currentThread);
                    while (m_threadInUpgradeableReadMode != null || m_threadInWriteMode != null || m_threadsWaitingForWriteLock.Any())
                    {
                        if (!Monitor.Wait(m_lockObject, timeout))
                        {
                            return false;
                        }
                    }
                    m_threadsWaitingForUpgradeableReadLock.Remove(currentThread);
                    m_threadInUpgradeableReadMode = new LockHolder(currentThread);
                }
            }
            return true;
        }

        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            return TryEnterWriteLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public bool TryEnterWriteLock(TimeSpan timeout)
        {
            lock (m_lockObject)
            {
                Thread currentThread = Thread.CurrentThread;
                if (IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }
                    m_threadInWriteMode.NumberOfLocks++;
                }
                else
                {
                    if (m_threadsInReadMode.Any(t => t.Thread == currentThread))
                    {
                        throw new LockRecursionException(StringResources.CannotAcquireWriteLockWhileReadLock);
                    }
                    else if (RecursionPolicy == LockRecursionPolicy.NoRecursion && TryingToGetRecursiveLock())
                    {
                        throw new LockRecursionException(StringResources.PolicyNotAllowRecursiveLocks);
                    }

                    m_threadsWaitingForWriteLock.Add(currentThread);
                    while (m_threadInWriteMode != null && m_threadInWriteMode.Thread != currentThread || m_threadsInReadMode.Any())
                    {
                        if (!Monitor.Wait(m_lockObject, timeout))
                        {
                            return false;
                        }
                    }
                    m_threadsWaitingForWriteLock.Remove(currentThread);
                    m_threadInWriteMode = new LockHolder(currentThread);
                }
            }
            return true;
        }

        private bool IsCurrentThreadHoldingLock(LockHolder holder, Thread currentThread = null)
        {
            if (currentThread == null)
            {
                currentThread = Thread.CurrentThread;
            }

            return holder != null && holder.Thread == currentThread;
        }

        private bool IsThreadNotCurrentLockHolder(LockHolder holder, Thread currentThread)
        {
            return holder != null && holder.Thread != currentThread;
        }

        private bool TryingToGetRecursiveLock()
        {
            Thread currentThread = Thread.CurrentThread;
            if (IsCurrentThreadHoldingLock(m_threadInWriteMode, currentThread)) return true;
            if (IsCurrentThreadHoldingLock(m_threadInUpgradeableReadMode, currentThread)) return true;
            return m_threadsInReadMode.Any(t => t.Thread == currentThread);
        }

        private class LockHolder
        {
            public LockHolder(Thread thread)
            {
                Thread = thread;
                NumberOfLocks = 1;
            }

            public int NumberOfLocks { get; set; }

            public Thread Thread { get; private set; }
        }
    }

#endif
}
