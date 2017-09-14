using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrodLib.Collections.BinaryTrees;
using System.Xml;
using System.IO;
using TestProject.TestHelpers;
using System.Diagnostics;
using System.Reflection;
using FrodLib.Extensions;
using System.Collections;

namespace TestProject
{
    /// <summary>
    /// Summary description for BinaryTreeTests
    /// </summary>
    [TestClass]
    public class BinaryTreeTests
    {
        public BinaryTreeTests()
        {

        }

        private const string TestDataFileName = "TestData/BinaryTreeTestData.xml";

        int testNumber;
        bool testHasBeenRun;

        [TestInitialize()]
        public void TestInitialize()
        {
            testNumber = 1;
            testHasBeenRun = false;
        }


        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("BinaryTree")]
        public void BinaryTreeAddRemoveTest()
        {

            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/AddRemoveTests/AddRemoveTest"))
            {
                BinaryTreeTestImpl.AddRemoveTest(node, typeof(BinaryTree<>), testNumber);
                testNumber++;
                testHasBeenRun = true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("BinaryTree")]
        public void BinaryTreeFindTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/FindTests/FindTest"))
            {
                BinaryTreeTestImpl.FindTest(node, typeof(BinaryTree<>), testNumber);

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
        [TestCategory("BinaryTree")]
        public void BinaryTreeAddRangeTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/AddRangeTests/AddRangeTest"))
            {
                BinaryTreeTestImpl.AddRangeTest(node, typeof(BinaryTree<>), testNumber);

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
        [TestCategory("BinaryTree")]
        public void BinaryTreeClearTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/ClearTests/ClearTest"))
            {
                BinaryTreeTestImpl.Clear(node, typeof(BinaryTree<>), testNumber);

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
