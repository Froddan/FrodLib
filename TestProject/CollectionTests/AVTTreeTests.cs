using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrodLib.Collections.BinaryTrees;
using System.Xml;
using TestProject.TestHelpers;

namespace TestProject
{
    [TestClass]
    public class AVTTreeTests
    {
        private const string TestDataFileName = "TestData/BinaryTreeTestData.xml";
        int testNumber;
        bool testHasBeenRun;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            testNumber = 1;
            testHasBeenRun = false;
        }

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("AVLTree")]
        public void AVTLTreeAddRemoveTest()
        {

            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/AddRemoveTests/AddRemoveTest"))
            {
                BinaryTreeTestImpl.AddRemoveTest(node, typeof(AVLTree<>), testNumber);
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
        [TestCategory("AVLTree")]
        public void AVTLTreeFindTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/FindTests/FindTest"))
            {
                BinaryTreeTestImpl.FindTest(node, typeof(AVLTree<>), testNumber);

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
        [TestCategory("AVLTree")]
        public void AVTTreeAddRangeTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/AddRangeTests/AddRangeTest"))
            {
                BinaryTreeTestImpl.AddRangeTest(node, typeof(AVLTree<>), testNumber);

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
        [TestCategory("AVLTree")]
        public void AVTTreeClearTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/BinaryTreeTestData/ClearTests/ClearTest"))
            {
                BinaryTreeTestImpl.Clear(node, typeof(AVLTree<>), testNumber);

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
