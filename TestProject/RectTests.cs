using System.Xml;
using FrodLib;
using FrodLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.TestHelpers;
using System.Linq;

namespace TestProject
{
    [TestClass]
    public class RectTests
    {
        private const string TestDataFileName = "TestData/RectTestData.xml";
        private readonly RectConverter converter = new RectConverter();
        private bool testHasBeenRun;
        private int testNumber;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            testNumber = 1;
            testHasBeenRun = false;
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectContainsTest()
        {
            PointConverter pointConverter = new PointConverter();
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/ContainTests/ContainTest"))
            {
                Rect startRect = GetRect(node, "StartRect");
                var containsNode = node["Contains"];
                Assert.IsNotNull(containsNode, "Node 'Contains' is missing" + TestHelper.AppendTestID(testNumber));

                string contains = string.Empty;

                bool result = false;
                if (containsNode.HasAttribute("value"))
                {
                    var unionValue = containsNode.Attributes["value"].Value;

                    int numberOfDelimiters = unionValue.Count(c => c == ';');
                    if (numberOfDelimiters == 1)
                    {
                        Point point = (Point)pointConverter.ConvertFrom(unionValue);
                        result = startRect.Contains(point);

                        contains = "Point: " + point;
                    }
                    else if (numberOfDelimiters == 3)
                    {
                        Rect rect = (Rect)converter.ConvertFrom(unionValue);
                        result = startRect.Contains(rect);

                        contains = "Rect: " + rect;
                    }
                    else
                    {
                        Assert.Fail("Wrong number of expected delimiters (';'). Expected one or three delimiters" + TestHelper.AppendTestID(testNumber));
                    }
                }
                else if (containsNode.HasAttribute("x") && containsNode.HasAttribute("y"))
                {
                    double x = containsNode.Attributes["x"].Value.ToDouble();
                    double y = containsNode.Attributes["y"].Value.ToDouble();

                    result = startRect.Contains(x, y);

                    contains = "x: " + x + ",y: " + y;
                }
                else
                {
                    Assert.Fail("Contains node doesnt have the attr 'value' or the attributs 'x' and 'y'" + TestHelper.AppendTestID(testNumber));
                }

                var expectedResultNode = node["ExpectedResult"];
                Assert.IsNotNull(expectedResultNode, "Node 'ExpectedResult' is missing" + TestHelper.AppendTestID(testNumber));

                bool expectedResult = bool.Parse(expectedResultNode.Attributes["value"].Value);

                Assert.AreEqual(expectedResult, result, string.Format("Expeced a result of {0} when testing if rect {1} contains {2}, but received {3}{4}", expectedResult, startRect, contains, result, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectEqualsTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/EqualsTests/EqualsTest"))
            {
                Rect startRect = GetRect(node, "StartRect");
                Rect equalsRect = GetRect(node, "EqualsRect");

                bool result = startRect == equalsRect;

                var inflateNode = node["ExpectedResult"];
                Assert.IsNotNull(inflateNode, "Node 'ExpectedResult' is missing" + TestHelper.AppendTestID(testNumber));

                bool expectedResult = bool.Parse(inflateNode.Attributes["value"].Value);

                Assert.AreEqual(expectedResult, result, string.Format("Expeced a result of {0} when testing if rect {0} contains rect {2}, but received {3}{4}", expectedResult, startRect, equalsRect, result, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectInflateTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/InflateTests/InflateTest"))
            {
                Rect startRect = GetRect(node, "StartRect");

                var inflateNode = node["InflateWith"];
                Assert.IsNotNull(inflateNode, "Node 'InflateWith' is missing" + TestHelper.AppendTestID(testNumber));

                double width = inflateNode.Attributes["width"].Value.ToDouble();
                double height = inflateNode.Attributes["height"].Value.ToDouble();

                startRect.Inflate(width, height);

                Rect expectedRect = GetRect(node, "ExpectedResultRect");

                Assert.AreEqual(expectedRect, startRect, string.Format("The inflated rect '{0}' doesn't match with the expected rect '{1}'{2}", startRect, expectedRect, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectIntersectTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/IntersectsTests/IntersectsTest"))
            {
                Rect startRect = GetRect(node, "StartRect");
                Rect intersectsRect = GetRect(node, "IntersectRect");
                
                startRect.Intersect(intersectsRect);

                Rect expectedRect = GetRect(node, "ExpectedResultRect");

                Assert.AreEqual(expectedRect, startRect, string.Format("The intersected rect '{0}' doesn't match with the expected rect '{1}'{2}", startRect, expectedRect, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectIntersectWidthTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/IntersectsWithTests/IntersectsWithTest"))
            {
                Rect startRect = GetRect(node, "StartRect");
                Rect intersectsRect = GetRect(node, "IntersectRect");

                bool result = startRect.IntersectsWith(intersectsRect);

                var inflateNode = node["ExpectedResult"];
                Assert.IsNotNull(inflateNode, "Node 'ExpectedResult' is missing" + TestHelper.AppendTestID(testNumber));

                bool expectedResult = bool.Parse(inflateNode.Attributes["value"].Value);

                Assert.AreEqual(expectedResult, result, string.Format("Expeced a result of {0} when testing if rect {0} intersects with rect {2} but received {3}{4}", expectedResult, startRect, intersectsRect, result, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectOffsetTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/OffsetTests/OffsetTest"))
            {
                Rect startRect = GetRect(node, "StartRect");

                var inflateNode = node["OffsetWith"];
                Assert.IsNotNull(inflateNode, "Node 'OffsetWith' is missing" + TestHelper.AppendTestID(testNumber));

                double offsetx = inflateNode.Attributes["offsetx"].Value.ToDouble();
                double offsety = inflateNode.Attributes["offsety"].Value.ToDouble();

                startRect.Offset(offsetx, offsety);

                Rect expectedRect = GetRect(node, "ExpectedResultRect");

                Assert.AreEqual(expectedRect, startRect, string.Format("The offseted rect '{0}' doesn't match with the expected rect '{1}'{2}", startRect, expectedRect, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectScaleTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/ScaleTests/ScaleTest"))
            {
                Rect startRect = GetRect(node, "StartRect");

                var scaleNode = node["Scale"];
                Assert.IsNotNull(scaleNode, "Node 'Scale' is missing" + TestHelper.AppendTestID(testNumber));

                double scaleX = scaleNode.Attributes["scalex"].Value.ToDouble();
                double scaleY = scaleNode.Attributes["scaley"].Value.ToDouble();

                startRect.Scale(scaleX, scaleY);

                Rect expectedRect = GetRect(node, "ExpectedResultRect");

                Assert.AreEqual(expectedRect, startRect, string.Format("The scaled rect '{0}' doesn't match with the expected rect '{1}'{2}", startRect, expectedRect, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Rect")]
        public void RectUnionTest()
        {
            PointConverter pointConverter = new PointConverter();

            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/RectTestData/UnionTests/UnionTest"))
            {
                Rect startRect = GetRect(node, "StartRect");

                var unionNode = node["Union"];
                Assert.IsNotNull(unionNode, "Node 'Union' is missing" + TestHelper.AppendTestID(testNumber));

                var unionValue = unionNode.Attributes["value"].Value;

                int numberOfDelimiters = unionValue.Count(c => c == ';');
                if (numberOfDelimiters == 1)
                {
                    Point point = (Point)pointConverter.ConvertFrom(unionValue);
                    startRect.Union(point);
                }
                else if (numberOfDelimiters == 3)
                {
                    Rect rect = (Rect)converter.ConvertFrom(unionValue);
                    startRect.Union(rect);
                }
                else
                {
                    Assert.Fail("Worng number of expected delimiters (';'). Expected one or three delimiters" + TestHelper.AppendTestID(testNumber));
                }

                Rect expectedRect = GetRect(node, "ExpectedResultRect");

                Assert.AreEqual(expectedRect, startRect, string.Format("The union rect '{0}' doesn't match with the expected rect '{1}'{2}", startRect, expectedRect, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        private Rect GetRect(XmlNode node, string nodeName)
        {
            XmlNode rectNode = node[nodeName];
            Assert.IsNotNull(rectNode, "Node '" + nodeName + "' is missing" + TestHelper.AppendTestID(testNumber));

            var rectString = rectNode.Attributes["value"].Value;
            if (!converter.CanConvertFrom(rectString.GetType())) Assert.Fail(string.Format("Cannot convert '{0}' to Rect{1}", rectString, TestHelper.AppendTestID(testNumber)));
            return (Rect)converter.ConvertFrom(rectString);
        }
    }
}