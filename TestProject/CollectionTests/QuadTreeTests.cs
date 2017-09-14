using System;
using System.Linq;
using System.Text;
using System.Xml;
using FrodLib.Collections.QuadTrees;
using FrodLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.TestHelpers;

namespace TestProject
{
    [TestClass]
    public class QuadTreeTests
    {
        private const string TestDataFileName = "TestData/QuadTreeTestData.xml";
        private RectConverter converter = new RectConverter();
        private bool testHasBeenRun;
        private int testNumber;

        [TestMethod]
        [TestCategory("Collections")]
        [TestCategory("QuadTree")]
        public void QuadTreeContainsTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/QuadTreeTestData/ContainsTests/ContainsTest"))
            {
                var quadTreeBoundaryNode = node["QuadTreeBoundarySize"];
                Assert.IsNotNull(quadTreeBoundaryNode, "QuadTreeBoundarySize node is missing" + TestHelper.AppendTestID(testNumber));

                Rect quadTreeBoundary = GetRect(quadTreeBoundaryNode, "value");

                QuadTree<object> quadTree = new QuadTree<object>(quadTreeBoundary);
                RunQuadTreeInstructions(node, quadTree);
                
                var containsNode = node["Contains"];
                Assert.IsNotNull(containsNode, "Contains node is missing" + TestHelper.AppendTestID(testNumber));

                object value = TestHelper.GetObjectValue(containsNode, testNumber);

                bool result = quadTree.Contains(value);

                var expectedResultNode = node["ExpectedResult"];
                Assert.IsNotNull(expectedResultNode, "ExpectedResult node is missing" + TestHelper.AppendTestID(testNumber));

                bool expectedContainsResult = bool.Parse(expectedResultNode.Attributes["value"].Value);

                StringBuilder sb = new StringBuilder();
                if (expectedContainsResult)
                {
                    sb.Append("The quad tree doesn't contain the expected value of: ");
                    sb.Append(value);
                }
                else
                {
                    sb.Append("The quad tree shoudn't contain the value: ");
                    sb.Append(value);
                }

                Assert.AreEqual(expectedContainsResult, result, sb.ToString() + TestHelper.AppendTestID(testNumber));

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
        [TestCategory("QuadTree")]
        public void QuadTreeInsertRemoveTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/QuadTreeTestData/InsertRemoveTests/InsertRemoveTest"))
            {
                var quadTreeBoundaryNode = node["QuadTreeBoundarySize"];
                Assert.IsNotNull(quadTreeBoundaryNode, "QuadTreeBoundarySize node is missing" + TestHelper.AppendTestID(testNumber));

                Rect quadTreeBoundary = GetRect(quadTreeBoundaryNode, "value");

                QuadTree<object> quadTree = new QuadTree<object>(quadTreeBoundary);
                RunQuadTreeInstructions(node, quadTree);

                int expectedValue = int.Parse(node["ExpectedResult"].InnerText);

                Assert.AreEqual(expectedValue, quadTree.Count);

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
        [TestCategory("QuadTree")]
        public void QuadTreeMoveTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/QuadTreeTestData/MoveTests/MoveTest"))
            {
                var quadTreeBoundaryNode = node["QuadTreeBoundarySize"];
                Assert.IsNotNull(quadTreeBoundaryNode, "QuadTreeBoundarySize node is missing" + TestHelper.AppendTestID(testNumber));

                Rect quadTreeBoundary = GetRect(quadTreeBoundaryNode, "value");

                QuadTree<object> quadTree = new QuadTree<object>(quadTreeBoundary);
                RunQuadTreeInstructions(node, quadTree);

                var moveItemNode = node["ItemToMove"];
                Assert.IsNotNull(moveItemNode, "ItemToMove node is missing" + TestHelper.AppendTestID(testNumber));

                var expectedResultNode = node["ExpectedResult"];
                Assert.IsNotNull(expectedResultNode, "ExpectedResult node is missing" + TestHelper.AppendTestID(testNumber));

                Action<string, string> countNumbersInBoundary = (newOrOld, beforeOrAfter) =>
                {
                    string queryNodeName = "Query" + newOrOld + "Boundary";
                    var queryBoundaryNode = node[queryNodeName];
                    Assert.IsNotNull(queryBoundaryNode, queryNodeName + " node is missing" + TestHelper.AppendTestID(testNumber));

                    Rect queryRect = GetRect(queryBoundaryNode);
                    var numberBeforeMove = quadTree.Query(queryRect).Count;
                    int expectedNumberBefore = int.Parse(expectedResultNode.Attributes[newOrOld + beforeOrAfter].Value);
                    Assert.AreEqual(expectedNumberBefore, numberBeforeMove, newOrOld + " boundary " + beforeOrAfter + " move" + TestHelper.AppendTestID(testNumber));
                };

                object moveItem = TestHelper.GetObjectValue(moveItemNode, testNumber);
                Rect newLocationBoundary = GetRect(moveItemNode);

                countNumbersInBoundary("Old", "Before");
                countNumbersInBoundary("New", "Before");

                bool expectedMoveResult = bool.Parse(expectedResultNode.Attributes["moveResult"].Value);
                bool result = quadTree.MoveItem(moveItem, newLocationBoundary);

                Assert.AreEqual(expectedMoveResult, result, TestHelper.AppendTestID(testNumber));

                countNumbersInBoundary("Old", "After");
                countNumbersInBoundary("New", "After");

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
        [TestCategory("QuadTree")]
        public void QuadTreeQueryTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/QuadTreeTestData/QueryTests/QueryTest"))
            {
                var quadTreeBoundaryNode = node["QuadTreeBoundarySize"];
                Assert.IsNotNull(quadTreeBoundaryNode, "QuadTreeBoundarySize node is missing" + TestHelper.AppendTestID(testNumber));

                Rect quadTreeBoundary = GetRect(quadTreeBoundaryNode, "value");

                QuadTree<object> quadTree = new QuadTree<object>(quadTreeBoundary);
                RunQuadTreeInstructions(node, quadTree);

                var queryNode = node["Query"];
                Assert.IsNotNull(queryNode, "Query node is missing" + TestHelper.AppendTestID(testNumber));

                Rect query = GetRect(queryNode, "value");

                var result = quadTree.Query(query);

                var expectedResultsNode = node["ExpectedResult"];
                Assert.IsNotNull(expectedResultsNode, "ExpectedResult node is missing" + TestHelper.AppendTestID(testNumber));

                int expectedResultCount = int.Parse(expectedResultsNode.Attributes["count"].Value);

                Assert.AreEqual(expectedResultCount, result.Count, string.Format("The expected result count was {0} but the query found {1} items {2}", expectedResultCount, result.Count, TestHelper.AppendTestID(testNumber)));

                foreach (XmlNode resultValueNode in expectedResultsNode.ChildNodes)
                {
                    var expectedResultValue = TestHelper.GetObjectValue(resultValueNode, testNumber);
                    Assert.IsTrue(result.Contains(expectedResultValue), "The result doesn't contain the expected value: " + expectedResultValue + TestHelper.AppendTestID(testNumber));
                }

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
        [TestCategory("QuadTree")]
        public void QuadTreeResizeTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/QuadTreeTestData/ResizeTests/ResizeTest"))
            {
                var quadTreeBoundaryNode = node["QuadTreeBoundarySize"];
                Assert.IsNotNull(quadTreeBoundaryNode, "QuadTreeBoundarySize node is missing" + TestHelper.AppendTestID(testNumber));

                Rect quadTreeBoundary = GetRect(quadTreeBoundaryNode, "value");

                QuadTree<object> quadTree = new QuadTree<object>(quadTreeBoundary);
                RunQuadTreeInstructions(node, quadTree);

                var nodesOutOfBounds = quadTree.NodesOutOfBounds;
                var nodesInBounds = quadTree.QueryAllInBoundsItems().Count;

                var expectedResultNode = node["ExpectedResult"];
                Assert.IsNotNull(expectedResultNode, "ExpectedResult node is missing" + TestHelper.AppendTestID(testNumber));

                int expectedNumberOfNodesInBounds = int.Parse(expectedResultNode.Attributes["expecedInBounds"].Value);
                int expectedNumberOfNodesOutOfBounds = int.Parse(expectedResultNode.Attributes["expecedOutOfBounds"].Value);

                Rect expecedNewBoundary = GetRect(expectedResultNode, "expectedNewBoundary");

                Assert.AreEqual(expectedNumberOfNodesInBounds, nodesInBounds, TestHelper.AppendTestID(testNumber));
                Assert.AreEqual(expectedNumberOfNodesOutOfBounds, nodesOutOfBounds, TestHelper.AppendTestID(testNumber));
                Assert.AreEqual(expecedNewBoundary, quadTree.Boundary, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestInitialize()]
        public void QuadTreeTestInitialize()
        {
            testNumber = 1;
            testHasBeenRun = false;
        }

        private Rect GetRect(XmlNode node)
        {
            return GetRect(node, "itemBoundary");
        }

        private Rect GetRect(XmlNode node, string attrName)
        {
            XmlAttribute rectNode = node.Attributes[attrName];
            Assert.IsNotNull(rectNode, "Attribute '" + attrName + "' is missing on node: " + node.Name + TestHelper.AppendTestID(testNumber));

            var rectString = rectNode.Value;
            if (!converter.CanConvertFrom(rectString.GetType())) Assert.Fail(string.Format("Cannot convert '{0}' to Rect{1}", rectString, TestHelper.AppendTestID(testNumber)));
            return (Rect)converter.ConvertFrom(rectString);
        }

        private void RunQuadTreeInstructions(XmlNode node, QuadTree<object> quadTree)
        {
            var instructionNode = node["Instructions"];
            Assert.IsNotNull(instructionNode, "instruction node is missing" + TestHelper.AppendTestID(testNumber));
            foreach (XmlNode instruction in instructionNode.ChildNodes)
            {
                if (instruction.Name == "Resize")
                {
                    Rect resizeTo = GetRect(instruction, "value");
                    quadTree.ResizeBoundary(resizeTo);
                }
                else
                {
                    object value = TestHelper.GetObjectValue(instruction, testNumber);

                    bool instructionResult = false;
                    if (instruction.Name == "Insert")
                    {
                        Rect itemBoundary = GetRect(instruction);
                        instructionResult = quadTree.Insert(value, itemBoundary);
                    }
                    else if (instruction.Name == "Remove")
                    {
                        instructionResult = quadTree.Remove(value);
                    }
                    else if (instruction.Name == "Move")
                    {
                        Rect newLocationBoundary = GetRect(instruction);
                        instructionResult = quadTree.MoveItem(value, newLocationBoundary);
                    }
                    else
                    {
                        Assert.Fail("Unknown instruction");
                    }

                    bool expectedInstructionResult = bool.Parse(instruction.Attributes["instructionResult"].Value);
                    Assert.AreEqual(expectedInstructionResult, instructionResult, string.Format("The expected instruction result was {0} but received {1}{2}", expectedInstructionResult, instructionResult, TestHelper.AppendTestID(testNumber)));
                }
            }
        }
    }
}