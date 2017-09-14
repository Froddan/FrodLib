using System;
using FrodLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class UndoredoManagerTests
    {
        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void UndoRedoSimpleTest()
        {
            UndoRedoManager undoRedo = new UndoRedoManager();

            TestDC testDataClass = new TestDC(undoRedo);

            Assert.AreEqual(0,testDataClass.Prop1);
            Assert.AreEqual(null, testDataClass.Prop2);

            testDataClass.Prop1 = 42;
            testDataClass.Prop2 = "Hej";

            Assert.AreEqual(42, testDataClass.Prop1);
            Assert.AreEqual("Hej", testDataClass.Prop2);

            testDataClass.Prop1 = 100;
            testDataClass.Prop2 = "tja";

            Assert.AreEqual(100, testDataClass.Prop1);
            Assert.AreEqual("tja", testDataClass.Prop2);

            undoRedo.Undo();
            undoRedo.Undo();

            Assert.AreEqual(42, testDataClass.Prop1);
            Assert.AreEqual("Hej", testDataClass.Prop2);
           
            undoRedo.Redo();
            undoRedo.Redo();

            Assert.AreEqual(100, testDataClass.Prop1);
            Assert.AreEqual("tja", testDataClass.Prop2);

            undoRedo.Undo();
            undoRedo.Undo();
            undoRedo.Undo();
            undoRedo.Undo();

            Assert.AreEqual(0, testDataClass.Prop1);
            Assert.AreEqual(null, testDataClass.Prop2);

        }

        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void UndoRedoTransactionTest()
        {
            UndoRedoManager undoRedo = new UndoRedoManager();

            TestDC testDataClass = new TestDC(undoRedo);

            Assert.AreEqual(0, testDataClass.Prop1);
            Assert.AreEqual(null, testDataClass.Prop2);

            using (new UndoRedoTransaction(undoRedo))
            {
                testDataClass.Prop1 = 42;
                testDataClass.Prop2 = "Hej";
            }

            Assert.AreEqual(42, testDataClass.Prop1);
            Assert.AreEqual("Hej", testDataClass.Prop2);

            using (new UndoRedoTransaction(undoRedo))
            {
                testDataClass.Prop1 = 100;
                testDataClass.Prop2 = "tja";
            }

            Assert.AreEqual(100, testDataClass.Prop1);
            Assert.AreEqual("tja", testDataClass.Prop2);

            undoRedo.Undo();

            Assert.AreEqual(42, testDataClass.Prop1);
            Assert.AreEqual("Hej", testDataClass.Prop2);

            undoRedo.Redo();

            Assert.AreEqual(100, testDataClass.Prop1);
            Assert.AreEqual("tja", testDataClass.Prop2);

            undoRedo.Undo();
            undoRedo.Undo();

            Assert.AreEqual(0, testDataClass.Prop1);
            Assert.AreEqual(null, testDataClass.Prop2);

        }

        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void ThrowTest()
        {
            UndoRedoManager undoRedo = new UndoRedoManager();

            try
            {
                undoRedo.Undo();
                Assert.Fail();
            }
            catch(InvalidOperationException) { }

            TestDC testDC = new TestDC(undoRedo);
            testDC.Prop1 = 42;

            try
            {
                undoRedo.Undo();
                
            }
            catch (InvalidOperationException) 
            {
                Assert.Fail();
            }

            try
            {
                undoRedo.Redo();
                
            }
            catch (InvalidOperationException) { Assert.Fail(); }
            undoRedo.Clear();
            try
            {
                undoRedo.Undo();
                Assert.Fail();
            }
            catch (InvalidOperationException) { }

        }

        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void NoOperationsTest()
        {
            UndoRedoManager undoRedo = new UndoRedoManager();

            TestDC testDataClass = new TestDC(undoRedo);

            using (new UndoRedoTransaction(undoRedo))
            {
                testDataClass.Prop1 = 42;
                testDataClass.Prop2 = "Hej";
            }

            using (new UndoRedoTransaction(undoRedo))
            {
                testDataClass.Prop1 = 100;
                testDataClass.Prop2 = "tja";
            }

            Assert.AreEqual(2, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(0, undoRedo.NumberOfRedoOperations);

            undoRedo.Undo();

            Assert.AreEqual(1, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(1, undoRedo.NumberOfRedoOperations);

            undoRedo.Undo();

            Assert.AreEqual(0, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(2, undoRedo.NumberOfRedoOperations);

            undoRedo.Redo();

            Assert.AreEqual(1, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(1, undoRedo.NumberOfRedoOperations);

            undoRedo.Redo();

            Assert.AreEqual(2, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(0, undoRedo.NumberOfRedoOperations);

            undoRedo.Undo();
            undoRedo.Undo();

            testDataClass.Prop1 = 200;

            Assert.AreEqual(1, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(0, undoRedo.NumberOfRedoOperations);
        }

        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void ClearTest()
        {
            UndoRedoManager undoRedo = new UndoRedoManager();
            undoRedo.MaxItems = 10;
            TestDC testDataClass = new TestDC(undoRedo);

            testDataClass.Prop1 = 1;
            testDataClass.Prop1 = 2;
            testDataClass.Prop1 = 3;
            testDataClass.Prop1 = 4;
            testDataClass.Prop1 = 5;
            testDataClass.Prop1 = 6;
            testDataClass.Prop1 = 7;

            undoRedo.Undo();

            Assert.AreEqual(6, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(1, undoRedo.NumberOfRedoOperations);

            undoRedo.Clear();

            Assert.AreEqual(0, undoRedo.NumberOfUndoOperations);
            Assert.AreEqual(0, undoRedo.NumberOfRedoOperations);
        }

        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void EventRaiseTest()
        {
            bool undoRaised = false;
            bool redoRaised = false;

            UndoRedoManager undoRedo = new UndoRedoManager();
            undoRedo.UndoStackStatusChanged += (s, a) =>
            {
                undoRaised = true;
            };

            undoRedo.RedoStackStatusChanged += (s, a) =>
            {
                redoRaised = true;
            };


            TestDC testDataClass = new TestDC(undoRedo);
            testDataClass.Prop1 = 1;

            Assert.IsTrue(undoRaised, "Undo should have been raised");
            Assert.IsFalse(redoRaised, "Redo should NOT have been raised");

            undoRaised = false;
            redoRaised = false;

            undoRedo.Undo();

            Assert.IsTrue(undoRaised, "Undo should have been raised");
            Assert.IsTrue(redoRaised, "Redo should have been raised");

            undoRaised = false;
            redoRaised = false;

            undoRedo.Redo();

            Assert.IsTrue(undoRaised, "Undo should have been raised");
            Assert.IsTrue(redoRaised, "Redo should have been raised");

            undoRaised = false;
            redoRaised = false;

            undoRedo.Clear();

            Assert.IsTrue(undoRaised, "Undo should have been raised");
            Assert.IsFalse(redoRaised, "Redo should have been raised");
        }

        [TestMethod]
        [TestCategory("Undo/Redo")]
        public void MaintainSizeTest()
        {
            UndoRedoManager undoRedo = new UndoRedoManager();

            TestDC testDataClass = new TestDC(undoRedo);

            const int max = 5;

            undoRedo.MaxItems = max;

            testDataClass.Prop1 = 1;
            testDataClass.Prop1 = 2;
            testDataClass.Prop1 = 3;
            testDataClass.Prop1 = 4;
            testDataClass.Prop1 = 5;
            testDataClass.Prop1 = 6;
            testDataClass.Prop1 = 7;

            Assert.AreEqual(max, undoRedo.NumberOfUndoOperations);

            for(int i = 1 ; i<= max ; i++)
            {
                undoRedo.Undo();
                Assert.AreEqual(7 - i, testDataClass.Prop1);
            }

            undoRedo.Clear();

            undoRedo.MaxItems = 10;

            testDataClass.Prop1 = 1;
            testDataClass.Prop1 = 2;
            testDataClass.Prop1 = 3;
            testDataClass.Prop1 = 4;
            testDataClass.Prop1 = 5;
            testDataClass.Prop1 = 6;
            testDataClass.Prop1 = 7;

            Assert.AreEqual(7, undoRedo.NumberOfUndoOperations);

            undoRedo.MaxItems = max;

            Assert.AreEqual(max, undoRedo.NumberOfUndoOperations);

            for (int i = 1; i <= max; i++)
            {
                undoRedo.Undo();
                Assert.AreEqual(7 - i, testDataClass.Prop1);
            }
        }

        private class TestDC
        {
            private UndoRedoManager m_undoRedoManager;
            public TestDC(UndoRedoManager undoRedoManager)
            {
                m_undoRedoManager = undoRedoManager;
            }

            private int m_prop1;

            public int Prop1
            {
                get { return m_prop1; }
                set
                {
                    m_undoRedoManager.Push(x => this.Prop1 = x, m_prop1);
                    m_prop1 = value;
                }
            }

            private string m_prop2;

            public string Prop2
            {
                get { return m_prop2; }
                set
                {
                    m_undoRedoManager.Push(x => this.Prop2 = x, m_prop2);
                    m_prop2 = value;
                }
            }

        }
    }
}
