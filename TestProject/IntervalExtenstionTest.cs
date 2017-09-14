using FrodLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for IntervalExtenstionTest and is intended
    ///to contain all IntervalExtenstionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IntervalExtenstionTest
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        
        [TestMethod()]
        [TestCategory("Extensions")]
        public void IsInRangeTest()
        {
            Assert.IsTrue(2.IsInRange(1,3));
            Assert.IsTrue(3.IsInRange(1, 3));
            Assert.IsFalse(3.IsInRange(1, 3, false));
            Assert.IsFalse(4.IsInRange(1, 3));

            Assert.IsTrue(new DateTime(2013, 2, 27).IsInRange(new DateTime(2013, 2, 1), new DateTime(2013, 2, 28)));
            Assert.IsTrue(new DateTime(2013, 2, 28).IsInRange(new DateTime(2013, 2, 1), new DateTime(2013, 2, 28)));
            Assert.IsFalse(new DateTime(2013, 2, 27).IsInRange(new DateTime(2013, 2, 1), new DateTime(2013, 2, 27), false));
            Assert.IsFalse(new DateTime(2013, 3, 27).IsInRange(new DateTime(2013, 2, 1), new DateTime(2013, 2, 28)));
        }
    }
}
