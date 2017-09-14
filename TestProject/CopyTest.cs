using FrodLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestProject.TestDataClasses;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for CopyTest and is intended
    ///to contain all CopyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CopyTest
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

        const string City = "Stockholm";
        const string Street = "Secret Street";
        const int Zip = 12345;

        Address addressFrom;
        Address addressTo;

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
            addressFrom = new Address();
            addressTo = new Address();

            addressFrom.City = City;
            addressFrom.Street = Street;
            addressFrom.ZipCode = Zip;
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            addressFrom = null;
            addressTo = null;
        }
        //
        #endregion

        [TestMethod()]
        [TestCategory("CopyTo")]
        [TestCategory("Extensions")]
        public void CopyToTest()
        {
            Copy.CopyTo(addressFrom, addressTo);
            Assert.AreEqual(addressTo.Street, addressFrom.Street);
            Assert.AreEqual(addressTo.ZipCode, addressFrom.ZipCode);
            Assert.AreEqual(addressTo.City, addressFrom.City);
        }

        
        [TestMethod()]
        [TestCategory("CopyTo")]
        [TestCategory("Extensions")]
        public void CopyToWithAttributeTest()
        {
            Copy.CopyTo(addressFrom, addressTo, typeof(CopyAttribute));
            Assert.AreEqual(addressTo.Street, addressFrom.Street);
            Assert.AreEqual(addressTo.ZipCode, addressFrom.ZipCode);
            Assert.IsNull(addressTo.City);
        }
    }
}
