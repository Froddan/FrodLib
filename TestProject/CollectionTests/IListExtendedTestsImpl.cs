using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.TestHelpers;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using FrodLib.Extensions;
using FrodLib;

namespace TestProject.CollectionTests
{
    [TestClass]
    public abstract class IListExtendedTestsImpl<T> where T : IListExtended<object>
    {

        private const string TestDataFileName = "TestData/IListExtendedTestData.xml";

        private const string InstructionAddRange = "AddRange";
        private const string InstructionAdd = "Add";
        private const string InstructionInsertRange = "InsertRange";
        private const string InstructionInsert = "Insert";
        private const string InstructionRemoveRange = "RemoveRange";
        private const string InstructionRemove = "Remove";
        private const string InstructionRemoveAt = "RemoveAt";

        int testNumber;
        bool testHasBeenRun;
        Type testedType;

        [TestInitialize()]
        public void TestInitialize()
        {
            testedType = typeof(T);
            testNumber = 1;
            testHasBeenRun = false;
        }

        protected abstract T CreateInstance();

        [TestCategory("IListExtendedTests")]
        public virtual void AddRemoveInsertRangeTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/IListExtendedTestData/AddRemoveInsertRangeTests/AddRemoveInsertRangeTest"))
            {
                T collectionInstance = CreateInstance();

                var instructionNode = node["Instructions"];
                RunInstructions(collectionInstance, instructionNode);

                int expectedCount = int.Parse(node["ExpectedCount"].InnerText);

                Assert.AreEqual(expectedCount, collectionInstance.Count, TestHelper.AppendTestID(testNumber, testedType));

                IList<object> expectedRange = (IList<object>)TestHelper.GetObjectValue(node["ExpectedRange"], testNumber);

                Assert.IsTrue(CollectionExtensions.AreEquals(collectionInstance, expectedRange), TestHelper.AppendTestID(testNumber, testedType));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestCategory("IListExtendedTests")]
        public virtual void SetCapacityTest()
        {
            Assert.Inconclusive("Test not implemented");
        }

        [TestCategory("IListExtendedTests")]
        public virtual void ClearTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/IListExtendedTestData/ClearTests/ClearTest"))
            {
                T collectionInstance = CreateInstance();

                var instructionNode = node["Instructions"];
                RunInstructions(collectionInstance, instructionNode);

                int expectedCount = int.Parse(node["ExpectedCount1"].InnerText);
                Assert.AreEqual(expectedCount, collectionInstance.Count, TestHelper.AppendTestID(testNumber, testedType));

                collectionInstance.Clear();

                int expectedCount2 = int.Parse(node["ExpectedCount2"].InnerText);
                Assert.AreEqual(expectedCount2, collectionInstance.Count, TestHelper.AppendTestID(testNumber, testedType));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestCategory("IListExtendedTests")]
        public virtual void AddRemoveInsertItemTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/IListExtendedTestData/AddRemoveInsertItemsTests/AddRemoveInsertItemsTest"))
            {
                T collectionInstance = CreateInstance();

                var instructionNode = node["Instructions"];
                RunInstructions(collectionInstance, instructionNode);

                int expectedCount = int.Parse(node["ExpectedCount"].InnerText);

                Assert.AreEqual(expectedCount, collectionInstance.Count, TestHelper.AppendTestID(testNumber, testedType));

                IList<object> expectedRange = (IList<object>)TestHelper.GetObjectValue(node["ExpectedRange"], testNumber);

                Assert.IsTrue(CollectionExtensions.AreEquals(collectionInstance, expectedRange), "The content of the collection doesn't match the expected content" + TestHelper.AppendTestID(testNumber, testedType));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestCategory("IListExtendedTests")]
        public virtual void RandomAccessTest()
        {
            Assert.Inconclusive("Test not implemented");
        }

        [TestCategory("IListExtendedTests")]
        public virtual void ContainsTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/IListExtendedTestData/ContainsTests/ContainsTest"))
            {
                T collectionInstance = CreateInstance();

                var instructionNode = node["Instructions"];
                RunInstructions(collectionInstance, instructionNode);

                object containsValue = TestHelper.GetObjectValue(node["ContainsValue"], testNumber);
                bool result = collectionInstance.Contains(containsValue);

                bool expectedResult = bool.Parse(node["ExpectedResult"].InnerText);
                
                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestCategory("IListExtendedTests")]
        public virtual void CopyToArrayTest()
        {
            Assert.Inconclusive("Test not implemented");
        }

        [TestCategory("IListExtendedTests")]
        public virtual void IndexOfTest()
        {
            Assert.Inconclusive("Test not implemented");
        }

        private void RunInstructions(T instance, XmlNode instructionsNode)
        {

            Assert.IsNotNull(instructionsNode, "instruction node is null" + TestHelper.AppendTestID(testNumber, testedType));
            foreach (XmlNode instruction in instructionsNode.ChildNodes)
            {
                string methodName = instruction.Name;

                switch (methodName)
                {
                    case InstructionAddRange:
                        object parsedValue = TestHelper.GetObjectValue(instruction, testNumber);
                        IList<object> collection = (IList<object>)parsedValue;
                        instance.AddRange(collection);
                        break;
                    case InstructionAdd:
                        parsedValue = TestHelper.GetObjectValue(instruction, testNumber);
                        instance.Add(parsedValue);
                        break;
                    case InstructionInsertRange:
                        parsedValue = TestHelper.GetObjectValue(instruction, testNumber);
                        int atIndex = int.Parse(instruction.Attributes["AtIndex"].Value);
                        collection = (IList<object>)parsedValue;
                        instance.InsertRange(atIndex, collection);
                        break;
                    case InstructionInsert:
                        parsedValue = TestHelper.GetObjectValue(instruction, testNumber);
                        atIndex = int.Parse(instruction.Attributes["AtIndex"].Value);
                        instance.Insert(atIndex, parsedValue);
                        break;
                    case InstructionRemoveRange:
                        if (instruction.Attributes["StartIndex"] != null)
                        {
                            int startIndex = int.Parse(instruction.Attributes["StartIndex"].Value);
                            int count = -1;
                            if (instruction.Attributes["Count"] != null)
                            {
                                count = int.Parse(instruction.Attributes["Count"].Value);
                            }
                            instance.RemoveRange(startIndex, count);
                        }
                        else
                        {
                            parsedValue = TestHelper.GetObjectValue(instruction, testNumber);
                            collection = (IList<object>)parsedValue;
                            instance.RemoveRange(collection);
                        }
                        break;
                    case InstructionRemove:
                        parsedValue = TestHelper.GetObjectValue(instruction, testNumber);
                        instance.Remove(parsedValue);
                        break;
                    case InstructionRemoveAt:
                        int removeAtIndex = (int)TestHelper.GetObjectValue(instruction, testNumber);
                        instance.RemoveAt(removeAtIndex);
                        break;
                    default:
                        Assert.Fail("Unknown collection instruction: " + methodName + TestHelper.AppendTestID(testNumber, testedType));
                        break;
                }
            }
        }
    }
}
