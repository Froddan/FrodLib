using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrodLib.Utils
{
    public class WorkerUnhandledExceptionEventArgs : EventArgs
    {
        public WorkerUnhandledExceptionEventArgs(Exception e)
        {
            Exception = e;
        }

        public Exception Exception { get; private set; }
    }

    /// <summary>
    /// Creates a worker queue that runs methods of separate threads
    /// </summary>
    public class WorkerQueue : IDisposable, IWorkerQueue
    {
        private readonly object lockObj = new object();
        private readonly object noBusyIndicatorLockObj = new object();
        private Thread[] _workers;
        private bool _continueConsume;
        protected Queue<IWorkerItem> queue = new Queue<IWorkerItem>();
        private int numberOfBusyThreads;
        private int numberOfWorkers;
        private readonly string m_workerQueueName;

        public event EventHandler<WorkerUnhandledExceptionEventArgs> WorkerUnhandledException;

        /// <summary>
        /// Gets the total number of workers available
        /// </summary>
        public int TotalNumberOfWorkers
        {
            get
            {
                return numberOfWorkers;
            }
        }

        public int CurrentAmountOfItemsInQueue
        {
            get
            {
                return queue.Count;
            }
        }

        /// <summary>
        /// Gets the number of threads that is currently busy processing tasks
        /// </summary>
        public int NumberOfBusyThreads
        {
            get
            {
                return numberOfBusyThreads;
            }
        }

        public bool IsRunning
        {
            get { return _continueConsume; }
        }

        /// <summary>
        /// Creates and starts a worker queue with the same number of worker threads as it exist logical processors on the machine
        /// </summary>
        public WorkerQueue()
            : this(Environment.ProcessorCount, true)
        {
        }

        /// <summary>
        /// Creates and starts a worker queue with the specified number of workers
        /// </summary>
        /// <param name="noOfWorkers">Indicates on how many worker threads the worker queue uses (minimum is one)</param>
        public WorkerQueue(int noOfWorkers) : this(noOfWorkers, true, null)
        {

        }

        /// <summary>
        /// Creates and starts a worker queue with the specified number of workers
        /// </summary>
        /// <param name="noOfWorkers">Indicates on how many worker threads the worker queue uses (minimum is one)</param>
        public WorkerQueue(int noOfWorkers, bool autoStart) : this(noOfWorkers, autoStart, null)
        {

        }

        /// <summary>
        /// Creates and starts a worker queue with the specified number of workers
        /// </summary>
        /// <param name="noOfWorkers">Indicates on how many worker threads the worker queue uses (minimum is one)</param>
        public WorkerQueue(int noOfWorkers, bool autoStart, string workerQueueName)
        {
            if (string.IsNullOrWhiteSpace(workerQueueName))
            {
                m_workerQueueName = string.Empty;
            }
            else
            {
                m_workerQueueName = workerQueueName.TrimEnd() + " - ";
            }
            if (noOfWorkers < 1)
            {
                noOfWorkers = 1;
            }
            numberOfWorkers = noOfWorkers;

            if (autoStart)
            {
                Start();
            }
        }

        ~WorkerQueue()
        {
            ShutDown(false);
        }

        /// <summary>
        /// Starts the worker queue with the number of workers, that was last specified. Either thru the constructor or thru Start(int numberOfWorkersThreads).
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Worker queue is already running. Please shut it down first before you try to start it</exception>
        public void Start()
        {
            Start(numberOfWorkers);
        }

        /// <summary>
        /// Starts the worker queue.
        /// </summary>
        /// <param name="numberOfWorkersThreads">The number of workers the queue should use</param>
        /// /// <exception cref="System.InvalidOperationException">Worker queue is already running. Please shut it down first before you try to start it</exception>
        public void Start(int numberOfWorkersThreads)
        {
            if (_continueConsume)
            {
                throw new InvalidOperationException(StringResources.WorkerQueueAlreadyRunning);
            }
            lock (lockObj)
            {
                _continueConsume = true;
                numberOfWorkers = numberOfWorkersThreads;
                _workers = new Thread[numberOfWorkers];

                for (int i = 0; i < numberOfWorkers; i++)
                {
                    _workers[i] = new Thread(Consume);
                    _workers[i].IsBackground = true;
                    _workers[i].Name = m_workerQueueName + " Queue worker " + (i + 1);
                    _workers[i].Start();
                }
            }
        }

        /// <summary>
        /// Shuts down the worker queue, and stops all workers after there tasks are finished.
        /// </summary>
        /// <param name="waitForWorkers">Indicate if it will wait for the threads to finish before it returns</param>
        public void ShutDown(bool waitForWorkers)
        {
            lock (lockObj)
            {
                queue.Clear();
                _continueConsume = false;
                for (int i = 0; i < _workers.Length; i++)
                {
                    queue.Enqueue(null);
                }
                Monitor.PulseAll(lockObj);
            }
            if (waitForWorkers)
            {
                for (int i = 0; i < _workers.Length; i++)
                {
                    _workers[i].Join();
                }
            }
        }

        /// <summary>
        /// Adds a new item to the worker queue and starts to run that item
        /// <para>when any worker threads becomes available</para>
        /// </summary>
        /// <param name="item">The action that the workers should run</param>
        /// <exception cref="InvalidOperationException">Is thrown if the worker queue has been shuted down</exception>
        public void EnqueueItem(Action item)
        {
            ArgumentValidator.IsNotNull(item, nameof(item));
            if (!_continueConsume)
            {
                throw new InvalidOperationException(StringResources.WorkerQueueShutedDown);
            }
            lock (lockObj)
            {
                queue.Enqueue(new ActionWorkerItem(item));
                Monitor.Pulse(lockObj);
            }
        }

        /// <summary>
        /// Adds a new item to the worker queue and starts to run that item
        /// <para>when any worker threads becomes available</para>
        /// </summary>
        /// <param name="item">The action that the workers should run</param>
        /// <exception cref="InvalidOperationException">Is thrown if the worker queue has been shuted down</exception>
        public void EnqueueItem(Delegate item, params object[] args)
        {
            ArgumentValidator.IsNotNull(item, nameof(item));
            if (!_continueConsume)
            {
                throw new InvalidOperationException(StringResources.WorkerQueueShutedDown);
            }
            lock (lockObj)
            {
                queue.Enqueue(new DelegateWorkerItem(item, args));
                Monitor.Pulse(lockObj);
            }
        }

        /// <summary>
        /// Adds a new item to the worker queue and starts to run that item
        /// <para>when any worker threads becomes available</para>
        /// </summary>
        /// <param name="item">The action that the workers should run</param>
        /// <exception cref="InvalidOperationException">Is thrown if the worker queue has been shuted down</exception>
        public void EnqueueItem(IWorkerItem item)
        {
            ArgumentValidator.IsNotNull(item, nameof(item));
            if (!_continueConsume)
            {
                throw new InvalidOperationException(StringResources.WorkerQueueShutedDown);
            }
            lock (lockObj)
            {
                queue.Enqueue(item);
                Monitor.Pulse(lockObj);
            }
        }

        private void Consume()
        {
            while (_continueConsume)
            {
                try
                {
                    IWorkerItem item;
                    lock (lockObj)
                    {
                        while (queue.Count == 0)
                        {
                            Monitor.Wait(lockObj);
                        }
                        item = queue.Dequeue();
                    }

                    if (item != null)
                    {
                        lock (noBusyIndicatorLockObj)
                        {
                            numberOfBusyThreads++;
                        }
                        item.Execute();
                        lock (noBusyIndicatorLockObj)
                        {
                            numberOfBusyThreads--;
                        }
                    }
                }
                catch (Exception e)
                {
                    RaiseWorkerUnhandledException(e);
                }
            }
        }

        private void RaiseWorkerUnhandledException(Exception e)
        {
            var handler = WorkerUnhandledException;
            if(handler != null)
            {
                handler(this, new WorkerUnhandledExceptionEventArgs(e));
            }
        }

        void IDisposable.Dispose()
        {
            ShutDown(true);
        }

        #region WorkerItems

        protected internal class ActionWorkerItem : IWorkerItem
        {
            readonly Action m_action;

            public ActionWorkerItem(Action action)
            {
                this.m_action = action;
            }

            public void Execute()
            {
                m_action();
            }
        }

        protected internal class DelegateWorkerItem : IWorkerItem
        {
            readonly Delegate m_method;
            readonly object[] m_args;

            public DelegateWorkerItem(Delegate method, params object[] args)
            {
                this.m_method = method;
                this.m_args = args;
            }

            public void Execute()
            {
                m_method.DynamicInvoke(m_args);
            }
        }

        #endregion WorkerItems
    }
}