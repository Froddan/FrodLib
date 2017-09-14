using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrodLib.Utils;

namespace FrodLib.Tests
{
    [TestClass]
    public class ArgumentValidatorTests
    {
        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsClassNullTest()
        {
            try
            {
                TestClass1 tc1 = null;
                ArgumentValidator.IsNotNull(tc1, "tc1");
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            {

            }

        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsClassNotNullTest()
        {

            TestClass1 tc1 = new TestClass1();
            ArgumentValidator.IsNotNull(tc1, "tc1");
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsStructNullTest()
        {
            try
            {
                TestStruct1? ts1 = null;
                ArgumentValidator.IsNotNull(ts1, "tc1");
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            {

            }
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsStructNotNullTest()
        {
            TestStruct1? ts1 = new TestStruct1();
            ArgumentValidator.IsNotNull(ts1, "tc1");
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsLessThenTest()
        {
            ArgumentValidator.IsLesserThen(5, "input", 10);

            try
            {
                ArgumentValidator.IsLesserThen(10, "input", 10);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            { }

            try
            {
                ArgumentValidator.IsLesserThen(15, "input", 10);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsLessThenOrEqualToTest()
        {
            ArgumentValidator.IsLesserThenOrEqual(5, "input", 10);

            ArgumentValidator.IsLesserThenOrEqual(10, "input", 10);

            try
            {
                ArgumentValidator.IsLesserThenOrEqual(15, "input", 10);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsGreaterThenTest()
        {
            ArgumentValidator.IsGreaterThen(15, "input", 10);

            try
            {
                ArgumentValidator.IsGreaterThen(10, "input", 10);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            { }

            try
            {
                ArgumentValidator.IsGreaterThen(5, "input", 10);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsGreaterThenOrEqualToTest()
        {
            ArgumentValidator.IsGreaterThenOrEqual(15, "input", 10);

            ArgumentValidator.IsGreaterThenOrEqual(10, "input", 10);

            try
            {
                ArgumentValidator.IsGreaterThenOrEqual(5, "input", 10);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException)
            { }
        }

        [TestMethod]
        [TestCategory("ArgumentValidator")]
        public void IsInRangeTest()
        {
            try
            {
                ArgumentValidator.IsInRange(0, "input", 1, 3);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException) { }
            ArgumentValidator.IsInRange(1, "input", 1, 3);
            ArgumentValidator.IsInRange(2, "input", 1, 3);
            ArgumentValidator.IsInRange(3, "input", 1, 3);
            try
            {
                ArgumentValidator.IsInRange(4, "input", 1, 3);
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException) { }
        }

        private class TestClass1
        {

        }

        private struct TestStruct1
        {

        }
    }
}
