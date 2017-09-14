using FrodLib;
using FrodLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace TestProject
{


    /// <summary>
    ///This is a test class for WorkerQueueTest and is intended
    ///to contain all WorkerQueueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WorkerQueueTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private const int Workers = 1;
        private IWorkerQueue worker;

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            worker = new WorkerQueue(Workers);
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (worker != null && worker.IsRunning)
            {
                worker.ShutDown(true);
            }
            worker = null;
        }

        #endregion


        /// <summary>
        ///A test for EnqueueItem
        ///</summary>
        [TestMethod()]
        [TestCategory("Queue")]
        [TestCategory("WorkerQueue")]
        public void EnqueueItemTest()
        {
            bool hasRunned = false;
            object lockObject = new object();
            Action item = () =>
            {
                lock (lockObject)
                {
                    hasRunned = true;
                    Monitor.Pulse(lockObject);
                }
            };
            worker.EnqueueItem(item);

            lock (lockObject)
            {
                while (!hasRunned)
                {
                    Monitor.Wait(lockObject);
                }
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        ///A test for ShutDown
        ///</summary>
        [TestMethod()]
        [TestCategory("Queue")]
        [TestCategory("WorkerQueue")]
        public void ShutDownTest()
        {
            bool waitForWorkers = true;
            worker.ShutDown(waitForWorkers);
            Assert.IsFalse(worker.IsRunning);
        }

        /// <summary>
        ///A test for Start
        ///</summary>
        [TestMethod()]
        [TestCategory("Queue")]
        [TestCategory("WorkerQueue")]
        public void StartTest()
        {
            bool waitForWorkers = true;
            worker.ShutDown(waitForWorkers);
            Assert.IsFalse(worker.IsRunning);

            worker.Start();
            Assert.IsTrue(worker.IsRunning);
            Assert.AreEqual(worker.TotalNumberOfWorkers, Workers);
            worker.ShutDown(true);
        }

        /// <summary>
        ///A test for Start
        ///</summary>
        [TestMethod()]
        [TestCategory("Queue")]
        [TestCategory("WorkerQueue")]
        public void StartWithSpecifiedNumberOfWOrkersTest()
        {
            bool waitForWorkers = true;
            worker.ShutDown(waitForWorkers);
            Assert.IsFalse(worker.IsRunning);

            int numberOfWorkers = worker.TotalNumberOfWorkers;

            worker.Start(numberOfWorkers +1);
            Assert.IsTrue(worker.IsRunning);
            Assert.AreEqual(worker.TotalNumberOfWorkers, numberOfWorkers +1);
            Assert.AreNotEqual(worker.TotalNumberOfWorkers, Workers);
            worker.ShutDown(false);
        }


        /// <summary>
        ///A test for number of busy threads
        ///</summary>
        [TestMethod()]
        [TestCategory("Queue")]
        [TestCategory("WorkerQueue")]
        public void BusyThreadsTest()
        {
            Assert.AreEqual(worker.NumberOfBusyThreads, 0);
            Action item = () =>
            {
                Thread.Sleep(1000);
            };
            worker.EnqueueItem(item);

            Thread.Sleep(500);
            Assert.AreEqual(worker.NumberOfBusyThreads, 1);
        }
    }
}
