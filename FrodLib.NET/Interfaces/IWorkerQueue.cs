using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    public interface IWorkerQueue
    {
        int TotalNumberOfWorkers
        {
            get;
        }

        int NumberOfBusyThreads
        {
            get;
        }

        bool IsRunning
        {
            get;
        }

        void Start();
        void Start(int numberOfWorkersThreads);
        void ShutDown(bool waitForWorkers);
        void EnqueueItem(Action item);
    }
}
