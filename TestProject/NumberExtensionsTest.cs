using FrodLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestProject.TestHelpers;
using System.Xml;
using System.Globalization;

namespace TestProject
{


    /// <summary>
    ///This is a test class for NumberExtensionsTest and is intended
    ///to contain all NumberExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NumberExtensionsTest
    {
        private const string TestDataFileName = "TestData/NumberExtensionTestData.xml";
        int testNumber;
        bool testHasBeenRun;

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

        [TestInitialize()]
        public void MyTestInitialize()
        {
            testNumber = 1;
            testHasBeenRun = false;
        }


        /// <summary>
        ///A test for IsNumber
        ///</summary>
        [TestMethod()]
        [TestCategory("Extensions")]
        public void IsNumberTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/NumberExtensionTestData/IsNumberTests/IsNumberTest"))
            {
                var objectNode = node["Object"];
                Assert.IsNotNull(objectNode, "object node could not be found" + TestHelper.AppendTestID(testNumber));
                object value = TestHelper.GetObjectValue(objectNode, testNumber);

                var resultNode = node["Result"];
                Assert.IsNotNull(objectNode, "Result node could not be found" + TestHelper.AppendTestID(testNumber));
                bool result = bool.Parse(resultNode.Attributes["value"].Value);
                string message = value.GetType().ToString() + " should ";
                message += (result) ? "" : "not ";
                message += "be a number";
                Assert.AreEqual(value.IsNumber(), result, message);

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        /// <summary>
        ///A test for ToDecimal
        ///</summary>
        [TestMethod()]
        [TestCategory("Extensions")]
        public void ToDecimalTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/NumberExtensionTestData/ToNumericThrowTests/ToNumericThrowTest"))
            {
                var objectNode = node["StringValue"];
                Assert.IsNotNull(objectNode, "StringValue node could not be found" + TestHelper.AppendTestID(testNumber));

                string attrValue = objectNode.Attributes["value"].Value;

                Assert.IsNotNull(attrValue, "Attribute 'value' could not be found" + TestHelper.AppendTestID(testNumber));

                var resultNode = node["Result"];
                decimal expectedResult = decimal.Parse(resultNode.Attributes["value"].Value, CultureInfo.InvariantCulture);
                Assert.IsNotNull(objectNode, "Result node could not be found" + TestHelper.AppendTestID(testNumber));
                bool shouldThrowException = bool.Parse(resultNode.Attributes["shouldThrowException"].Value);

                try
                {
                    decimal resultValue = attrValue.ToDecimal();
                    Assert.AreEqual(expectedResult, resultValue);
                }
                catch (InvalidCastException e)
                {
                    Assert.IsTrue(shouldThrowException, e.Message);
                }
                catch (FormatException e)
                {
                    Assert.IsTrue(shouldThrowException, e.Message);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        /// <summary>
        ///A test for ToDouble
        ///</summary>
        [TestMethod()]
        [TestCategory("Extensions")]
        public void ToDoubleTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/NumberExtensionTestData/ToNumericThrowTests/ToNumericThrowTest"))
            {
                var objectNode = node["StringValue"];
                Assert.IsNotNull(objectNode, "StringValue node could not be found" + TestHelper.AppendTestID(testNumber));

                string attrValue = objectNode.Attributes["value"].Value;

                Assert.IsNotNull(attrValue, "Attribute 'value' could not be found" + TestHelper.AppendTestID(testNumber));

                var resultNode = node["Result"];
                double expectedResult = double.Parse(resultNode.Attributes["value"].Value, CultureInfo.InvariantCulture);
                Assert.IsNotNull(objectNode, "Result node could not be found" + TestHelper.AppendTestID(testNumber));
                bool shouldThrowException = bool.Parse(resultNode.Attributes["shouldThrowException"].Value);

                try
                {
                    double resultValue = attrValue.ToDouble();
                    Assert.AreEqual(expectedResult, resultValue);
                }
                catch (InvalidCastException e)
                {
                    Assert.IsTrue(shouldThrowException, e.Message);
                }
                catch (FormatException e)
                {
                    Assert.IsTrue(shouldThrowException, e.Message);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        /// <summary>
        ///A test for ToDouble
        ///</summary>
        [TestMethod()]
        [TestCategory("Extensions")]
        public void TryToDecimalTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/NumberExtensionTestData/TryToNumericTests/TryToNumericTest"))
            {
                var objectNode = node["StringValue"];
                Assert.IsNotNull(objectNode, "StringValue node could not be found" + TestHelper.AppendTestID(testNumber));

                string attrValue = objectNode.Attributes["value"].Value;

                Assert.IsNotNull(attrValue, "Attribute 'value' could not be found" + TestHelper.AppendTestID(testNumber));

                var resultNode = node["Result"];
                decimal expectedResult = decimal.Parse(resultNode.Attributes["value"].Value, CultureInfo.InvariantCulture);
                Assert.IsNotNull(objectNode, "Result node could not be found" + TestHelper.AppendTestID(testNumber));
                bool shouldReturnValue = bool.Parse(resultNode.Attributes["shouldReturnValue"].Value);

                try
                {
                    decimal result;
                    bool success = attrValue.TryToDecimal(out result);

                    if (shouldReturnValue)
                    {
                        Assert.IsTrue(success);
                    }
                    else
                    {
                        Assert.IsFalse(success);
                    }
                    Assert.AreEqual(expectedResult, result);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        /// <summary>
        ///A test for ToDouble
        ///</summary>
        [TestMethod()]
        [TestCategory("Extensions")]
        public void TryToDoubleTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/NumberExtensionTestData/TryToNumericTests/TryToNumericTest"))
            {
                var objectNode = node["StringValue"];
                Assert.IsNotNull(objectNode, "StringValue node could not be found" + TestHelper.AppendTestID(testNumber));

                string attrValue = objectNode.Attributes["value"].Value;

                Assert.IsNotNull(attrValue, "Attribute 'value' could not be found" + TestHelper.AppendTestID(testNumber));

                var resultNode = node["Result"];
                double expectedResult = double.Parse(resultNode.Attributes["value"].Value, CultureInfo.InvariantCulture);
                Assert.IsNotNull(objectNode, "Result node could not be found" + TestHelper.AppendTestID(testNumber));
                bool shouldReturnValue = bool.Parse(resultNode.Attributes["shouldReturnValue"].Value);

                try
                {
                    double result;
                    bool success = attrValue.TryToDouble(out result);

                    if(shouldReturnValue)
                    {
                        Assert.IsTrue(success);
                    }
                    else
                    {
                        Assert.IsFalse(success);
                    }
                    Assert.AreEqual(expectedResult, result);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }
    }
}
