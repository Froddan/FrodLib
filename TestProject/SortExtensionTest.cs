using FrodLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TestProject.TestDataClasses;

namespace TestProject
{


    /// <summary>
    ///This is a test class for SortExtensionTest and is intended
    ///to contain all SortExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SortExtensionTest
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

        Address[] addresses;
        int[] collection;

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
            collection = new int[] { 2, 5, 4, 6, 3, 1 };
            addresses = new Address[] 
            { 
                new Address{ City = "Umeå", Street = "Centralgatan", ZipCode = 11154},
                new Address{ City = "Göteborg", Street = "Avenyn", ZipCode = 12354},
                new Address{ City = "Malmö", Street = "Malmö vägen", ZipCode = 12754},
                new Address{ City = "Stockholm", Street = "Strandvägen", ZipCode = 12546},
                new Address{ City = "Jönköping", Street = "Elmia vägen", ZipCode = 19312},
                new Address{ City = "Stockholm", Street = "Ringvägen", ZipCode = 12545}
            };
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            addresses = null;
            collection = null;
        }
        //
        #endregion

        [TestMethod()]
        [TestCategory("Extensions")]
        public void QuickSortAsendingTest()
        {
            collection.QuickSort(i => i);
            for (int i = 0; i < collection.Length; i++)
            {
                Assert.AreEqual(collection[i], i + 1);
            }

            addresses.QuickSort(a => a.Street);
            Assert.AreEqual(addresses[0].Street, "Avenyn");
            Assert.AreEqual(addresses[1].Street, "Centralgatan");
            Assert.AreEqual(addresses[2].Street, "Elmia vägen");
            Assert.AreEqual(addresses[3].Street, "Malmö vägen");
            Assert.AreEqual(addresses[4].Street, "Ringvägen");
            Assert.AreEqual(addresses[5].Street, "Strandvägen");

            addresses.QuickSort(a => a.City);
            Assert.AreEqual(addresses[0].City, "Göteborg");
            Assert.AreEqual(addresses[1].City, "Jönköping");
            Assert.AreEqual(addresses[2].City, "Malmö");
            Assert.AreEqual(addresses[3].City, "Stockholm");
            Assert.AreEqual(addresses[4].City, "Stockholm");
            Assert.AreEqual(addresses[5].City, "Umeå");
        }

        [TestMethod()]
        [TestCategory("Extensions")]
        public void QuickSortDescendingTest()
        {
            collection.QuickSort(i => i, true);
            for (int i = 0, j = collection.Length; i < collection.Length; i++, j--)
            {
                Assert.AreEqual(collection[i], j);
            }

            addresses.QuickSort(a => a.Street, true);
            Assert.AreEqual(addresses[5].Street, "Avenyn");
            Assert.AreEqual(addresses[4].Street, "Centralgatan");
            Assert.AreEqual(addresses[3].Street, "Elmia vägen");
            Assert.AreEqual(addresses[2].Street, "Malmö vägen");
            Assert.AreEqual(addresses[1].Street, "Ringvägen");
            Assert.AreEqual(addresses[0].Street, "Strandvägen");

            addresses.QuickSort(a => a.City, true);
            Assert.AreEqual(addresses[5].City, "Göteborg");
            Assert.AreEqual(addresses[4].City, "Jönköping");
            Assert.AreEqual(addresses[3].City, "Malmö");
            Assert.AreEqual(addresses[2].City, "Stockholm");
            Assert.AreEqual(addresses[1].City, "Stockholm");
            Assert.AreEqual(addresses[0].City, "Umeå");
        }
    }
}
