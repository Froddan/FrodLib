using FrodLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for ObjectExtensionTest and is intended
    ///to contain all ObjectExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ThrowIfNullTest
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
        public void ThrowOnNullTest()
        {
            object obj = null;

            try
            {
                obj.IsNotNull(nameof(obj));
                Assert.Fail("No exception was throwned");
            }
            catch (ArgumentNullException)
            { }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod()]
        [TestCategory("Extensions")]
        public void NoneThrowOnNullTest()
        {
            object obj = new object();

            try
            {
                obj.IsNotNull(nameof(obj));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }
    }
}
