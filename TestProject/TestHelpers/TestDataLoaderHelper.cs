using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TestProject.TestHelpers
{
    internal static class TestDataLoaderHelper
    {
        internal static XmlNodeList GetNodes(string testDataFileName, string nodes)
        {

            // Check if the created file exists in the deployment directory
            if (!File.Exists(testDataFileName))
            {
                string defaultTestDataFileName = testDataFileName;
                testDataFileName = Path.GetFileName(testDataFileName);
                if(!File.Exists(testDataFileName))
                {
                    Assert.Fail("deployment failed: " + defaultTestDataFileName + " did not get deployed");
                }
            }
            
            XmlDocument doc = new XmlDocument();
            doc.Load(testDataFileName);

            return doc.SelectNodes(nodes);
        }

        internal static object LoadComplexTypeFromXML(XmlNode complexTypeNode)
        {
            Assert.IsNotNull(complexTypeNode, "Couldn't find node ComplexType");

            var type = complexTypeNode.Attributes["type"].Value;
            var complexType = Type.GetType(type);
            var initializerNodes = complexTypeNode.SelectNodes("ConstructorInitializer/ConstructorValue");
            object[] initializerValues = new object[initializerNodes.Count];
            for (int i = 0; i < initializerNodes.Count; i++)
            {
                object value;
                XmlNode initializerNode = initializerNodes[i];
                if (initializerNode.Attributes["complexType"] != null && bool.Parse(initializerNode.Attributes["complexType"].Value))
                {
                    value = LoadComplexTypeFromXML(initializerNode.SelectSingleNode("ComplexType"));
                }
                else
                {
                    string attrValue = initializerNode.Attributes["value"].Value;
                    string attrType = initializerNode.Attributes["type"].Value;
                    Assert.IsNotNull(attrValue, "Attribute value can't be null");
                    Assert.IsNotNull(attrType, "Attribute type can't be null");
                    value = Convert.ChangeType(attrValue, Type.GetType(attrType));
                }
                initializerValues[i] = value;
            }
            var instance = Activator.CreateInstance(complexType, initializerValues);

            foreach (XmlNode propertyNode in complexTypeNode.SelectNodes("Properties/Property"))
            {
                object value;
                if (propertyNode.Attributes["complexType"] != null && bool.Parse(propertyNode.Attributes["complexType"].Value))
                {
                    value = LoadComplexTypeFromXML(propertyNode.SelectSingleNode("ComplexType"));
                }
                else
                {
                    string attrValue = propertyNode.Attributes["value"].Value;
                    string attrType = propertyNode.Attributes["type"].Value;

                    Assert.IsNotNull(attrValue, "Attribute value can't be null");
                    Assert.IsNotNull(attrType, "Attribute type can't be null");
                    value = Convert.ChangeType(attrValue, Type.GetType(attrType));
                }

                string propertyName = propertyNode.Attributes["propertyName"].Value;
                var property = complexType.GetProperty(propertyName);
                Assert.IsNotNull(property, string.Format("Property: '{0}' doesn't exist in type: '{1}'", propertyName, complexType));
                property.SetValue(instance, value, null);
            }

            return instance;
        }


        //internal static Delegate ParseExpressionFromXMLToFunc(XmlNode expressionNode, IEnumerable<Assembly> asmContainingTypesForExpression = null)
        //{
        //    ExpressionSerializer serizlizer;
        //    if(asmContainingTypesForExpression != null)
        //    {
        //        TypeResolver typeResolver = new TypeResolver(asmContainingTypesForExpression.Distinct());
        //        serizlizer = new ExpressionSerialization.ExpressionSerializer(typeResolver);
        //    }
        //    else
        //    {
        //        serizlizer = new ExpressionSerialization.ExpressionSerializer();
        //    }
            
        //    Expression exp = serizlizer.Deserialize(XElement.Parse(expressionNode.OuterXml));
        //    var lambda = exp as LambdaExpression;
        //    return lambda.Compile();
        //}

        internal static object LoadRangeFromXML(XmlNode rangeNode)
        {
            Type rangeType = Type.GetType(rangeNode.Attributes["RangeType"].Value);
            Type nonGenericListType = typeof(List<>);
            Type genericListType = nonGenericListType.MakeGenericType(typeof(object));
            IList collectionInstance = (IList)Activator.CreateInstance(genericListType);
            foreach (XmlNode itemNode in rangeNode.ChildNodes)
            {
                string attrValue = itemNode.Attributes["value"].Value;
                object itemValue = Convert.ChangeType(attrValue, rangeType);
                collectionInstance.Add(itemValue);
            }
            return collectionInstance;
        }
    }
}
