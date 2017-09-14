using FrodLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for CollectionExtensionsTest and is intended
    ///to contain all CollectionExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CollectionExtensionsTest
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
        [TestCategory("Collections")]
        [TestCategory("Extensions")]
        public void CollectionAreEqualsTest()
        {
            List<int> col1 = new List<int>() { 1, 2, 3, 4, 5 };
            int[] col2 = { 1, 2,3,4,5 };

            Assert.IsTrue(col1.AreEquals(col2));

            col1 = new List<int>() { 1, 2, 3, 4, 5 };
            col2 = new int[] { 1, 3, 2, 4, 5 };

            Assert.IsFalse(col1.AreEquals(col2));

            col1 = new List<int>() { 1, 2, 3, 4, 5 };
            col2 = new int[] { 1, 2, 3, 4 };

            Assert.IsFalse(col1.AreEquals(col2));
        }
    }
}
