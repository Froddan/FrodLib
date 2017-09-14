using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.TestHelpers;
using System.Xml;
using FrodLib;

namespace TestProject
{
    [TestClass]
    public class FractionTests
    {
        private const string TestDataFileName = "TestData/FractionTestData.xml";

        int testNumber;
        bool testHasBeenRun;

        [TestInitialize()]
        public void TestInitialize()
        {
            
            testNumber = 1;
            testHasBeenRun = false;
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionDoubleConstructorTest()
        {
            Fraction tmpFrac = new Fraction(1.5);
            Assert.AreEqual(3, tmpFrac.Numerator);
            Assert.AreEqual(2, tmpFrac.Denominator);

            tmpFrac = new Fraction(0.8);
            Assert.AreEqual(4, tmpFrac.Numerator);
            Assert.AreEqual(5, tmpFrac.Denominator);

            tmpFrac = new Fraction(1 / 6.0d);
            Assert.AreEqual(1, tmpFrac.Numerator);
            Assert.AreEqual(6, tmpFrac.Denominator);
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionAdditionTest()
        {

            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/AdditionTests/AdditionTest"))
            {

                Fraction ActualResult = Fraction.Zero;
                Fraction frac1 = NodeToFraction(node["Fraction1"]);

                XmlNode valueNode;
                var fraction2Node = node["Fraction2"];
                if (fraction2Node != null)
                {
                    Fraction frac2 = NodeToFraction(fraction2Node);
                    ActualResult = frac1 + frac2;
                }
                else if ((valueNode = node["Value"]) != null)
                {
                    int value = int.Parse(valueNode.InnerText);
                    ActualResult = frac1 + value;
                }
                else
                {
                    Assert.Fail("Node: 'Fraction2' or 'Value' doesn't exist in node: " + node.Name + TestHelper.AppendTestID(testNumber));
                }


                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));

                Assert.AreEqual(ExpectedResult.GetReduced(), ActualResult, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionSubtractionTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/SubtractionTests/SubtractionTest"))
            {
                Fraction ActualResult = Fraction.Zero;
                Fraction frac1 = NodeToFraction(node["Fraction1"]);

                XmlNode valueNode;
                var fraction2Node = node["Fraction2"];
                if (fraction2Node != null)
                {
                    Fraction frac2 = NodeToFraction(fraction2Node);
                    ActualResult = frac1 - frac2;
                }
                else if ((valueNode = node["Value"]) != null)
                {
                    int value = int.Parse(valueNode.InnerText);
                    ActualResult = frac1 - value;
                }
                else
                {
                    Assert.Fail("Node: 'Fraction2' or 'Value' doesn't exist in node: " + node.Name + TestHelper.AppendTestID(testNumber));
                }


                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));

                Assert.AreEqual(ExpectedResult.GetReduced(), ActualResult, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionMultiplicationTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/MultiplicationTests/MultiplicationTest"))
            {
                Fraction ActualResult = Fraction.Zero;
                Fraction frac1 = NodeToFraction(node["Fraction1"]);

                XmlNode valueNode;
                var fraction2Node = node["Fraction2"];
                if (fraction2Node != null)
                {
                    Fraction frac2 = NodeToFraction(fraction2Node);
                    ActualResult = frac1 * frac2;
                }
                else if ((valueNode = node["Value"]) != null)
                {
                    int value = int.Parse(valueNode.InnerText);
                    ActualResult = frac1 * value;
                }
                else
                {
                    Assert.Fail("Node: 'Fraction2' or 'Value' doesn't exist in node: " + node.Name + TestHelper.AppendTestID(testNumber));
                }

                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));

                Assert.AreEqual(ExpectedResult.GetReduced(), ActualResult, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionDivisionTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/DivisionTests/DivisionTest"))
            {
                Fraction ActualResult = Fraction.Zero;
                Fraction frac1 = NodeToFraction(node["Fraction1"]);

                XmlNode valueNode;
                var fraction2Node = node["Fraction2"];
                if (fraction2Node != null)
                {
                    Fraction frac2 = NodeToFraction(fraction2Node);
                    ActualResult = frac1 / frac2;
                }
                else if ((valueNode = node["Value"]) != null)
                {
                    int value = int.Parse(valueNode.InnerText);
                    ActualResult = frac1 / value;
                }
                else
                {
                    Assert.Fail("Node: 'Fraction2' or 'Value' doesn't exist in node: " + node.Name + TestHelper.AppendTestID(testNumber));
                }

                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));

                Assert.AreEqual(ExpectedResult.GetReduced(), ActualResult, TestHelper.AppendTestID(testNumber));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionReductionTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/ReductionTests/ReductionTest"))
            {
                Fraction frac = NodeToFraction(node["Fraction"]);
                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));

                Assert.AreEqual(ExpectedResult, frac.GetReduced(), TestHelper.AppendTestID(testNumber));
                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionToDenomitorTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/ToDenomitorTests/ToDenomitorTest"))
            {
                Fraction frac = NodeToFraction(node["Fraction"]);
                int toDenom = int.Parse(node["ToDenomitor"].InnerText);
                Fraction actualResult = frac.ToDenominator(toDenom);
                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));

                Assert.AreEqual(ExpectedResult, actualResult, TestHelper.AppendTestID(testNumber));
                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionReciprocalTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/ReciprocalTests/ReciprocalTest"))
            {
                Fraction frac = NodeToFraction(node["Fraction"]);
                Fraction actualResult = frac.GetReciprocal();

                Fraction ExpectedResult = NodeToFraction(node["ExpectedResult"].SelectSingleNode("Fraction"));


                Assert.AreEqual(ExpectedResult, actualResult, TestHelper.AppendTestID(testNumber));
                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        private const string CEquals = "Equals";
        private const string NotEquals = "NotEquals";
        private const string LesserThen = "LesserThen";
        private const string GreaterThen = "GreaterThen";
        private const string GreaterThenOrEqual = "GreaterThenOrEqual";
        private const string LesserThenOrEqual = "LesserThenOrEqual";

        [TestMethod]
        [TestCategory("Fraction")]
        public void FractionComparisonTest()
        {
            foreach (XmlNode node in TestDataLoaderHelper.GetNodes(TestDataFileName, "/FractionTestData/ComparisonTests/ComparisonTest"))
            {
                Fraction frac1 = NodeToFraction(node["Fraction1"]);
                Fraction frac2 = NodeToFraction(node["Fraction2"]);

                string comparer = node["Comparer"].InnerText;
                bool actualResult = false;
                switch (comparer)
                {
                    case CEquals: actualResult = frac1 == frac2; break;
                    case NotEquals: actualResult = frac1 != frac2; break;
                    case LesserThen: actualResult = frac1 < frac2; break;
                    case GreaterThen: actualResult = frac1 > frac2; break;
                    case GreaterThenOrEqual: actualResult = frac1 >= frac2; break;
                    case LesserThenOrEqual: actualResult = frac1 <= frac2; break;
                    default: Assert.Fail(string.Format("Comparer: {0} is not supported. Please use: ['{1}, {2}, {3}, {4}, {5}, {6}'] instead {7}", comparer, CEquals, NotEquals, LesserThen, GreaterThen, GreaterThenOrEqual, LesserThenOrEqual, TestHelper.AppendTestID(testNumber))); break;
                }

                bool expectedResult = bool.Parse(node["ExpectedResult"].InnerText);

                Assert.AreEqual(expectedResult, actualResult, string.Format("{0} {1} {2} did not meet the expected result of: {3}{4}", frac1, comparer, frac2, expectedResult, TestHelper.AppendTestID(testNumber)));

                testNumber++;
                testHasBeenRun |= true;
            }

            if (!testHasBeenRun)
            {
                Assert.Inconclusive("No test data exist");
            }
        }

        private Fraction NodeToFraction(XmlNode node)
        {
            XmlNode numeratorNode = node["Numerator"];
            XmlNode denominatorNode = node["Denominator"];

            Assert.IsNotNull(numeratorNode, string.Format("Numerator doesn't exist in node: {0}{1}", node.Name, TestHelper.AppendTestID(testNumber)));
            Assert.IsNotNull(denominatorNode, string.Format("Denominator doesn't exist in node: {0}{1}", node.Name, TestHelper.AppendTestID(testNumber)));

            int numerator = int.Parse(numeratorNode.InnerText);
            int denominator = int.Parse(denominatorNode.InnerText);
            return new Fraction(numerator, denominator);
        }
    }
}
