using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.TestHelpers;
using System.Xml;
using FrodLib.Collections;

namespace TestProject
{
    /// <summary>
    /// Summary description for SkipListTests
    /// </summary>
    [TestClass]
    public class SkipListTests
    {
        public SkipListTests()
        {
        }

        private const string TestDataFileName = "TestData/SkipListTestData.xml";

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
        public void TestInitialize()
        {
            testNumber = 1;
            testHasBeenRun = false;
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("SkipList")]
        public void SkipListIndexOfTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/SkipListTestData/IndexOfTests/IndexOfTest"))
            {
                Type skipListType;
                object skipList = CreateSkipListObject(node, testNumber, out skipListType);

                var instructionNode = node["Instructions"];
                Assert.IsNotNull(instructionNode, "instruction node is null" + TestHelper.AppendTestID(testNumber));
                foreach (XmlNode instruction in instructionNode.ChildNodes)
                {
                    var method = skipListType.GetMethod(instruction.Name);

                    Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                    object value = TestHelper.GetObjectValue(instruction, testNumber);
                    method.Invoke(skipList, new object[] { value });
                }

                var IndexOfMethod = skipListType.GetMethod("IndexOf");
                Assert.IsNotNull(IndexOfMethod, "Couldn't find method: 'IndexOf' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                var findIndexOfNode = node["FindIndexOf"];
                Assert.IsNotNull(findIndexOfNode, "FindIndexOf node is missing" + TestHelper.AppendTestID(testNumber));

                object findValue = TestHelper.GetObjectValue(findIndexOfNode, testNumber);

                var foundIndex = (int)IndexOfMethod.Invoke(skipList, new object[] { findValue });

                int expectedIndex = int.Parse(node["Result"].InnerText);

                Assert.AreEqual(expectedIndex, foundIndex, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("SkipList")]
        public void SkipListClearTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/SkipListTestData/ClearTests/ClearTest"))
            {
                Type skipListType;
                object skipList = CreateSkipListObject(node, testNumber, out skipListType);

                var instructionNode = node["Instructions"];
                Assert.IsNotNull(instructionNode, "instruction node is null" + TestHelper.AppendTestID(testNumber));
                foreach (XmlNode instruction in instructionNode.ChildNodes)
                {
                    var method = skipListType.GetMethod(instruction.Name);

                    Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                    object value = TestHelper.GetObjectValue(instruction, testNumber);
                    method.Invoke(skipList, new object[] { value });
                }

                var clearMethod = skipListType.GetMethod("Clear");
                Assert.IsNotNull(clearMethod, "Couldn't find method: 'Clear' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                clearMethod.Invoke(skipList, null);

                int expectedCount = int.Parse(node["Result"].InnerText);
                var countPInfo = skipListType.GetProperty("Count");

                Assert.IsNotNull(countPInfo, string.Format("Property: 'Count' doesn't exist in type: '{0}'{1}", skipListType, TestHelper.AppendTestID(testNumber)));

                object actualCount = countPInfo.GetValue(skipList, null);
                Assert.AreEqual(expectedCount, actualCount, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("SkipList")]
        public void SkipListAddRangeTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/SkipListTestData/AddRangeTests/AddRangeTest"))
            {
                Type skipListType;
                object skipList = CreateSkipListObject(node, testNumber, out skipListType);

                Type collectionType;
                object collection = CreateSkipListObject(node, testNumber, out collectionType);

                var addMethod = collectionType.GetMethod("Add");
                Assert.IsNotNull(addMethod, "Couldn't find method: 'Add' in type: '" + collectionType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                var addValuesNode = node["AddValues"];
                Assert.IsNotNull(addValuesNode, "Couldn't find node 'AddValues'" + TestHelper.AppendTestID(testNumber));
                foreach (XmlNode valueNode in addValuesNode.ChildNodes)
                {
                    object value = TestHelper.GetObjectValue(valueNode, testNumber);
                    addMethod.Invoke(collection, new object[] { value });
                }

                var addRangeMethod = skipListType.GetMethod("AddRange");
                Assert.IsNotNull(addRangeMethod, "Couldn't find method: 'AddRange' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                addRangeMethod.Invoke(skipList, new object[] { collection });

                var countProperty = skipListType.GetProperty("Count");
                Assert.IsNotNull(countProperty, "Couldn't find property: 'Count' on type:" + skipListType.ToString() + TestHelper.AppendTestID(testNumber));

                var resultNode = node["Result"];
                Assert.IsNotNull(resultNode, "Couldn't find node 'Result'" + TestHelper.AppendTestID(testNumber));

                int expectedCount = int.Parse(resultNode.Attributes["value"].Value);
                int count = (int)countProperty.GetValue(skipList, null);

                Assert.AreEqual(expectedCount, count, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("SkipList")]
        public void SkipListAddRemoveTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/SkipListTestData/AddRemoveTests/AddRemoveTest"))
            {
                Type skipListType;
                object skipList = CreateSkipListObject(node, testNumber, out skipListType);

                var instructionNode = node["Instructions"];
                Assert.IsNotNull(instructionNode, "instruction node is null" + TestHelper.AppendTestID(testNumber));
                foreach (XmlNode instruction in instructionNode.ChildNodes)
                {
                    var method = skipListType.GetMethod(instruction.Name);

                    Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                    object value = TestHelper.GetObjectValue(instruction, testNumber);
                    method.Invoke(skipList, new object[] { value });
                }

                int expectedCount = int.Parse(node["Result"].InnerText);
                var countPInfo = skipListType.GetProperty("Count");

                Assert.IsNotNull(countPInfo, string.Format("Property: 'Count' doesn't exist in type: '{0}'{1}", skipListType, TestHelper.AppendTestID(testNumber)));

                object actualCount = countPInfo.GetValue(skipList, null);
                Assert.AreEqual(expectedCount, actualCount, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("SkipList")]
        public void SkipListContainsTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/SkipListTestData/ContainsTests/ContainsTest"))
            {
                Type skipListType;
                object skipList = CreateSkipListObject(node, testNumber, out skipListType);

                var instructionNode = node["Instructions"];
                Assert.IsNotNull(instructionNode, "instruction node is null" + TestHelper.AppendTestID(testNumber));
                foreach (XmlNode instruction in instructionNode.ChildNodes)
                {
                    var method = skipListType.GetMethod(instruction.Name);

                    Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                    object value = TestHelper.GetObjectValue(instruction, testNumber);
                    method.Invoke(skipList, new object[] { value });
                }

                int expectedCount = int.Parse(node["Result"].InnerText);
                var countPInfo = skipListType.GetProperty("Count");

                Assert.IsNotNull(countPInfo, string.Format("Property: 'Count' doesn't exist in type: '{0}'{1}", skipListType, TestHelper.AppendTestID(testNumber)));

                object actualCount = countPInfo.GetValue(skipList, null);
                Assert.AreEqual(expectedCount, actualCount, TestHelper.AppendTestID(testNumber));

                var containsMethod = skipListType.GetMethod("Contains");

                Assert.IsNotNull(containsMethod, "Couldn't find method: 'Contains' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));
                var containsNode = node["Contains"];
                object containsValue = TestHelper.GetObjectValue(containsNode, testNumber);
                var containsResult = containsMethod.Invoke(skipList, new object[] { containsValue });
                bool expectedContainsResult = bool.Parse(node["ContainsResult"].InnerText);

                Assert.AreEqual(expectedContainsResult, containsResult, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("SkipList")]
        public void SkipListValueByIndexTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/SkipListTestData/GetByIndexTests/GetByIndexTest"))
            {
                Type skipListType;
                object skipList = CreateSkipListObject(node, testNumber, out skipListType);

                var instructionNode = node["Instructions"];
                Assert.IsNotNull(instructionNode, "instruction node is null" + TestHelper.AppendTestID(testNumber));
                foreach (XmlNode instruction in instructionNode.ChildNodes)
                {
                    var method = skipListType.GetMethod(instruction.Name);

                    Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                    object value = TestHelper.GetObjectValue(instruction, testNumber);
                    method.Invoke(skipList, new object[] { value });
                }

                var indexMethod = skipListType.GetMethod("get_Item");
                Assert.IsNotNull(indexMethod, "Couldn't find method: 'get_Item' in type: '" + skipListType.ToString() + "'" + TestHelper.AppendTestID(testNumber));
                int index = int.Parse(node["AccessIndex"].InnerText);

                object foundValue = indexMethod.Invoke(skipList, new object[] { index });
                var expectedValueNode = node["ExpectedFindValue"];
                object expectedValueToFind = TestHelper.GetObjectValue(expectedValueNode, testNumber);

                Assert.AreEqual(expectedValueToFind, foundValue, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        private static object CreateSkipListObject(XmlNode node, int testNumber, out Type skipListType)
        {
            Type nonGenericTreeType = typeof(SkipList<>);
            var BinaryTreeGenericTypeNode = node["SkipListGenericType"];
            Assert.IsNotNull(BinaryTreeGenericTypeNode, "SkipListGenericType doesn't exist" + TestHelper.AppendTestID(testNumber));
            var genericType = Type.GetType(BinaryTreeGenericTypeNode.Attributes["value"].Value);
            skipListType = nonGenericTreeType.MakeGenericType(genericType);

            return Activator.CreateInstance(skipListType);
        }
    }
}
