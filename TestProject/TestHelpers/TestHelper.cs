using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.TestHelpers
{
    internal static class TestHelper
    {
        [DebuggerStepThrough]
        internal static string AppendTestID(int testNumber)
        {
            return " (Test Data: " + testNumber + ")";
        }

        [DebuggerStepThrough]
        internal static string AppendTestID(int testNumber, Type type)
        {
            return " (Test Data: " + testNumber + ", Type: "+type.Name+")";
        }

        internal static object GetObjectValue(XmlNode node, int testNumber)
        {
            Assert.IsNotNull(node, "Value node is missing" + TestHelper.AppendTestID(testNumber));

            object value;
            if (node.Attributes["complexType"] != null && bool.Parse(node.Attributes["complexType"].Value))
            {
                value = TestDataLoaderHelper.LoadComplexTypeFromXML(node.SelectSingleNode("ComplexType"));
            }
            else if(node.Attributes["IsRange"] != null && bool.Parse(node.Attributes["IsRange"].Value))
            {
                value = TestDataLoaderHelper.LoadRangeFromXML(node.SelectSingleNode("Range"));
            }
            else
            {
                string attrValue = node.Attributes["value"].Value;
                string attrType = node.Attributes["type"].Value;

                Assert.IsNotNull(attrValue, "Attribute 'value' could not be found" + TestHelper.AppendTestID(testNumber));
                Assert.IsNotNull(attrType, "Attribute 'type' could not be found" + TestHelper.AppendTestID(testNumber));
                value = Convert.ChangeType(attrValue, Type.GetType(attrType));
            }
            return value;
        }
    }
}
