using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if !USE_FROD_READ_WRITE_LOCK_IMPLEMENTATION
using FrodReaderWriterLock = System.Threading.ReaderWriterLockSlim;
using LockRecursionPolicy = System.Threading.LockRecursionPolicy;
#endif

#if USE_FROD_READ_WRITE_LOCK_IMPLEMENTATION
namespace TestProject
{


    [TestClass]
    public class ReadWriteLockTests
    {
        private const int SleepTime = 200;

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void ReadLockTest()
        {
            FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();
            object lockObject = new object();
            int numberOfReadLocks = 0;
            Exception[] exceptions = new Exception[10];

            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread((state) =>
                {
                    int index = (int)state;
                    try
                    {
                        readWriteLock.EnterReadLock();
                        lock (lockObject) numberOfReadLocks++;
                        Thread.Sleep(SleepTime);
                        Assert.AreEqual(threads.Length, numberOfReadLocks, "Failed on thread index: " + index);
                        Thread.Sleep(SleepTime);
                    }
                    catch (Exception e)
                    {
                        exceptions[index] = e;
                    }
                    finally
                    {
                        lock (lockObject) numberOfReadLocks--;
                        readWriteLock.ExitReadLock();
                    }

                });
                threads[i].Start(i);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            for (int i = 0; i < exceptions.Length; i++)
            {
                if (exceptions[i] != null)
                {
                    throw exceptions[i];
                }
            }
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void WriteLockTest()
        {
            FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();
            object lockObject = new object();
            int numberOfWriteLocks = 0;

            Exception[] exceptions = new Exception[10];
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread((obj) =>
                {
                    int index = (int)obj;
                    try
                    {
                        readWriteLock.EnterWriteLock();
                        lock (lockObject) numberOfWriteLocks++;
                        Thread.Sleep(SleepTime);
                        Assert.AreEqual(1, numberOfWriteLocks);
                        Thread.Sleep(SleepTime);
                    }
                    catch (Exception e)
                    {
                        exceptions[index] = e;
                    }
                    finally
                    {
                        lock (lockObject) numberOfWriteLocks--;
                        readWriteLock.ExitWriteLock();
                    }
                });
                threads[i].Start(i);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            for (int i = 0; i < exceptions.Length; i++)
            {
                if (exceptions[i] != null)
                {
                    throw exceptions[i];
                }
            }
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void ReadWriteLockTest()
        {
            FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();
            object lockObject = new object();

            int readThreads = 5;
            int writeThreads = 5;

            int numberOfReadLocks = 0;
            int numberOfWriteLocks = 0;

            int readValue = 0;

            Exception[] exceptions = new Exception[readThreads + writeThreads];
            Thread[] threads = new Thread[readThreads + writeThreads];

            //read
            for (int i = 0; i < readThreads; i++)
            {
                threads[i] = new Thread((obj) =>
                {
                    int index = (int)obj;
                    try
                    {
                        readWriteLock.EnterReadLock();
                        lock (lockObject) numberOfReadLocks++;
                        Thread.Sleep(SleepTime);
                        Assert.AreEqual(readThreads, numberOfReadLocks);
                        Assert.AreEqual(0, numberOfWriteLocks);
                        Assert.AreEqual(0, readValue);
                        Thread.Sleep(SleepTime);
                    }
                    catch (Exception e)
                    {
                        exceptions[index] = e;
                    }
                    finally
                    {
                        lock (lockObject) numberOfReadLocks--;
                        readWriteLock.ExitReadLock();
                    }
                });
            }

            //write
            for (int i = readThreads; i < readThreads + writeThreads; i++)
            {
                threads[i] = new Thread((obj) =>
                {
                    int index = (int)obj;
                    try
                    {
                        Thread.Sleep(50);
                        readWriteLock.EnterWriteLock();
                        lock (lockObject) numberOfWriteLocks++;
                        readValue++;
                        Assert.AreEqual(1, numberOfWriteLocks);
                        Assert.AreEqual(0, numberOfReadLocks);
                    }
                    catch (Exception e)
                    {
                        exceptions[index] = e;
                    }
                    finally
                    {
                        lock (lockObject) numberOfWriteLocks--;
                        readWriteLock.ExitWriteLock();
                    }
                });
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(i);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            for (int i = 0; i < exceptions.Length; i++)
            {
                if (exceptions[i] != null)
                {
                    throw exceptions[i];
                }
            }
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void WriteReadLockTest()
        {
            FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();
            object lockObject = new object();

            int readThreads = 5;
            int writeThreads = 5;

            int numberOfReadLocks = 0;
            int numberOfWriteLocks = 0;

            int readValue = 0;

            Exception[] exceptions = new Exception[readThreads + writeThreads];
            Thread[] threads = new Thread[readThreads + writeThreads];

            //read
            for (int i = 0; i < readThreads; i++)
            {
                threads[i] = new Thread((obj) =>
                {
                    int index = (int)obj;
                    try
                    {
                        Thread.Sleep(50);
                        readWriteLock.EnterReadLock();
                        lock (lockObject) numberOfReadLocks++;
                        Thread.Sleep(SleepTime);
                        Assert.AreEqual(readThreads, numberOfReadLocks);
                        Assert.AreEqual(0, numberOfWriteLocks);
                        Assert.AreEqual(readValue, writeThreads);
                        Thread.Sleep(SleepTime);
                    }
                    catch (Exception e)
                    {
                        exceptions[index] = e;
                    }
                    finally
                    {
                        lock (lockObject) numberOfReadLocks--;
                        readWriteLock.ExitReadLock();
                    }
                });
            }

            //write
            for (int i = readThreads; i < readThreads + writeThreads; i++)
            {
                threads[i] = new Thread((obj) =>
                {
                    int index = (int)obj;
                    try
                    {
                        readWriteLock.EnterWriteLock();
                        lock (lockObject) numberOfWriteLocks++;

                        Thread.Sleep(SleepTime);

                        readValue++;
                        Assert.AreEqual(1, numberOfWriteLocks);
                        Assert.AreEqual(0, numberOfReadLocks);
                    }
                    catch (Exception e)
                    {
                        exceptions[index] = e;
                    }
                    finally
                    {
                        lock (lockObject) numberOfWriteLocks--;
                        readWriteLock.ExitWriteLock();
                    }
                });
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(i);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            for (int i = 0; i < exceptions.Length; i++)
            {
                if (exceptions[i] != null)
                {
                    throw exceptions[i];
                }
            }
        }

        private void RecursiveRead(LockRecursionPolicy policy, bool shouldThrowException)
        {
            try
            {
                FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock(policy);
                readWriteLock.EnterReadLock();
                readWriteLock.EnterReadLock();
                Assert.AreEqual(2, readWriteLock.RecursiveReadCount);
                readWriteLock.ExitReadLock();
                readWriteLock.ExitReadLock();

                if (shouldThrowException)
                {
                    Assert.Fail("Should have throwned exception");
                }
            }
            catch (LockRecursionException e)
            {
                if (!shouldThrowException)
                {
                    Assert.Fail("Should not throw exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Caught exception: " + e.Message);
            }
        }

        private void RecursiveWrite(LockRecursionPolicy policy, bool shouldThrowException)
        {
            try
            {
                FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock(policy);
                readWriteLock.EnterWriteLock();
                readWriteLock.EnterWriteLock();
                Assert.AreEqual(2, readWriteLock.RecursiveWriteCount);
                readWriteLock.ExitWriteLock();
                readWriteLock.ExitWriteLock();

                if (shouldThrowException)
                {
                    Assert.Fail("Should have throwned exception");
                }
            }
            catch (LockRecursionException e)
            {
                if (!shouldThrowException)
                {
                    Assert.Fail("Should not throw exception: " + e.Message);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Caught exception: " + e.Message);
            }
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void ReqursiveReadLockTest()
        {
            RecursiveRead(LockRecursionPolicy.NoRecursion, true);
            RecursiveRead(LockRecursionPolicy.SupportsRecursion, false);
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void ReqursiveWriteLockTest()
        {
            RecursiveWrite(LockRecursionPolicy.NoRecursion, true);
            RecursiveWrite(LockRecursionPolicy.SupportsRecursion, false);
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void UpgradeableReadLockTest()
        {
            try
            {
                FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();
                readWriteLock.EnterUpgradeableReadLock();
                Assert.IsTrue(readWriteLock.IsUpgradeableReadLockHeld);
                Assert.AreEqual(1, readWriteLock.RecursiveUpgradeCount);
                readWriteLock.EnterWriteLock();
                Assert.IsTrue(readWriteLock.IsWriteLockHeld);
                readWriteLock.ExitWriteLock();
                readWriteLock.ExitUpgradeableReadLock();
            }
            catch (Exception e)
            {
                Assert.Fail("Caught exception: " + e.Message);
            }
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void ReadToWriteLockTest()
        {
            try
            {
                FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();
                readWriteLock.EnterReadLock();
                Assert.IsTrue(readWriteLock.IsReadLockHeld);
                Assert.AreEqual(1, readWriteLock.CurrentReadCount);
                readWriteLock.EnterWriteLock();
                Assert.IsTrue(readWriteLock.IsWriteLockHeld);
                readWriteLock.ExitWriteLock();
                readWriteLock.ExitUpgradeableReadLock();
                Assert.Fail("Should have throwned exception");
            }
            catch (LockRecursionException)
            {

            }
            catch (Exception e)
            {
                Assert.Fail("Caught exception: " + e.Message);
            }
        }

        [TestMethod]
        [TestCategory("ReadWriteLock")]
        public void WriteBeforeUpgradeRecursiveLockTest()
        {
            try
            {
                FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock(LockRecursionPolicy.SupportsRecursion);
                readWriteLock.EnterWriteLock();
                Assert.IsTrue(readWriteLock.IsWriteLockHeld);
                readWriteLock.EnterUpgradeableReadLock();
                Assert.IsTrue(readWriteLock.IsUpgradeableReadLockHeld);
                readWriteLock.ExitUpgradeableReadLock();
                Assert.IsFalse(readWriteLock.IsUpgradeableReadLockHeld);
                readWriteLock.ExitWriteLock();
                Assert.IsFalse(readWriteLock.IsWriteLockHeld);
            }
            catch (Exception e)
            {
                Assert.Fail("Caught exception: " + e.Message);
            }
        }
    }
}

#endif
