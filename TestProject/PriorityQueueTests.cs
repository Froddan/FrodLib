using System;
using FrodLib.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class PriorityQueueTests
    {
        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("Queue")]
        [TestCategory("PriorityQueue")]
        public void PriorityQueueEnqueueTest()
        {
            PriorityQueue<string> queue = new PriorityQueue<string>();

            queue.Enqueue("Middle", 2);
            queue.Enqueue("Low", 3);
            queue.Enqueue("High", 1);

            Assert.AreEqual(3, queue.Count);
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("Queue")]
        [TestCategory("PriorityQueue")]
        public void PriorityQueueDequeueTest()
        {
            PriorityQueue<string> queue = new PriorityQueue<string>();

            queue.Enqueue("Middle", 2);
            queue.Enqueue("Low", 3);
            queue.Enqueue("High", 1);

            Assert.AreEqual(3, queue.Count);

            Assert.AreEqual("High", queue.Dequeue());
            Assert.AreEqual("Middle", queue.Dequeue());
            Assert.AreEqual("Low", queue.Dequeue());

            Assert.AreEqual(0, queue.Count);

            queue.Enqueue("Middle", 2);
            queue.Enqueue("Low", 3);
            queue.Enqueue("High", 1);
            queue.Enqueue("Low2", 3);
            queue.Enqueue("Middle2", 2);
            queue.Enqueue("High2", 0);

            Assert.AreEqual(6, queue.Count);

            Assert.AreEqual("High", queue.Dequeue());
            Assert.AreEqual("High2", queue.Dequeue());
            Assert.AreEqual("Middle", queue.Dequeue());
            Assert.AreEqual("Middle2", queue.Dequeue());
            Assert.AreEqual("Low", queue.Dequeue());
            Assert.AreEqual("Low2", queue.Dequeue());

            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("Queue")]
        [TestCategory("PriorityQueue")]
        public void PriorityQueuePeekTest()
        {
            PriorityQueue<string> queue = new PriorityQueue<string>();

            queue.Enqueue("Middle", 2);
            queue.Enqueue("Low", 3);
            queue.Enqueue("High", 1);

            Assert.AreEqual("High", queue.Peek());
            Assert.AreEqual(3, queue.Count);
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("Queue")]
        [TestCategory("PriorityQueue")]
        public void PriorityQueueContainsTest()
        {
            PriorityQueue<string> queue = new PriorityQueue<string>();

            queue.Enqueue("Middle", 2);
            queue.Enqueue("Low", 3);
            queue.Enqueue("High", 1);

            Assert.IsTrue(queue.Contains("Middle"));
            Assert.IsFalse(queue.Contains("Doesn't exist"));
        }
    }
}
