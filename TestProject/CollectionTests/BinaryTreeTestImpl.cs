using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace TestProject.TestHelpers
{
    class BinaryTreeTestImpl
    {

        public static void AddRemoveTest(XmlNode node, Type binaryTreeType, int testNumber)
        {

            Type treeType;
            object binaryTree = CreateTreeObject(node, binaryTreeType, testNumber,out treeType);

            var instructionNode = node["Instructions"];
            Assert.IsNotNull(instructionNode, "instruction node is null" + TestHelper.AppendTestID(testNumber));
            foreach (XmlNode instruction in instructionNode.ChildNodes)
            {
                var method = treeType.GetMethod(instruction.Name);

                Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + treeType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                object value = TestHelper.GetObjectValue(instruction, testNumber);
                method.Invoke(binaryTree, new object[] { value });
            }

            int expectedValue = int.Parse(node["Result"].InnerText);
            var countPInfo = treeType.GetProperty("Count");

            Assert.IsNotNull(countPInfo, string.Format("Property: 'Count' doesn't exist in type: '{0}'{1}", treeType, TestHelper.AppendTestID(testNumber)));
            object actualValue = countPInfo.GetValue(binaryTree, null);
            Assert.AreEqual(expectedValue, actualValue, TestHelper.AppendTestID(testNumber));
        }

        
        public static void FindTest(XmlNode node, Type binaryTreeType, int testNumber)
        {
            Type treeType;
            object binaryTree = CreateTreeObject(node, binaryTreeType,testNumber, out treeType);

            var initInstructionNode = node["Instructions"];
            Assert.IsNotNull(initInstructionNode, "Couldn't find node 'InitInstructions'" + TestHelper.AppendTestID(testNumber));
            foreach (XmlNode instruction in initInstructionNode.ChildNodes)
            {
                var method = treeType.GetMethod(instruction.Name);

                Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + treeType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                object value = TestHelper.GetObjectValue(instruction, testNumber);
                method.Invoke(binaryTree, new object[] { value });
            }

            var findMethod = treeType.GetMethod("Find");
            Assert.IsNotNull(findMethod, "Couldn't find method: 'Find' on type:" + treeType.ToString() + TestHelper.AppendTestID(testNumber));

            XmlNode findNode = node["FindElement"];
            Assert.IsNotNull(findNode, "Couldn't find node 'FindElement'" + TestHelper.AppendTestID(testNumber));
            Type valueType = null;
            object findValue = TestHelper.GetObjectValue(findNode, testNumber);

            object foundItem = findMethod.Invoke(binaryTree, new object[] { findValue });

            var resultNode = node["Result"];
            if (resultNode.Attributes["IsValue"] != null && bool.Parse(resultNode.Attributes["IsValue"].Value))
            {
                if (valueType != null)
                {
                    var parseMethod = valueType.GetMethod("Parse", new Type[] { typeof(String) });
                    Assert.IsNotNull(parseMethod);
                    object expectedValue = parseMethod.Invoke(null, new object[] { resultNode.InnerText });
                    Assert.AreEqual(expectedValue, foundItem, TestHelper.AppendTestID(testNumber));
                }

            }
            else
            {
                bool expectedNull = bool.Parse(resultNode.Attributes["ExpectNull"].Value);

                if (expectedNull)
                {
                    Assert.IsNull(foundItem);
                }
                else
                {
                    Assert.IsNotNull(foundItem);
                }
            }
        }

        private static object CreateTreeObject(XmlNode node, Type binaryTreeType, int testNumber, out Type treeType)
        {
            Type nonGenericTreeType = binaryTreeType;
            var BinaryTreeGenericTypeNode = node["BinaryTreeGenericType"];
            Assert.IsNotNull(BinaryTreeGenericTypeNode, "BinaryTreeGenericType doesn't exist" + TestHelper.AppendTestID(testNumber));
            var genericType = Type.GetType(BinaryTreeGenericTypeNode.Attributes["value"].Value);
            Type[] typeArgs = Type.GetTypeArray(new object[] { genericType });
            treeType = nonGenericTreeType.MakeGenericType(genericType);

            return Activator.CreateInstance(treeType);
        }

        internal static void Clear(XmlNode node, Type binaryTreeType, int testNumber)
        {
            Type treeType;
            object binaryTree = CreateTreeObject(node, binaryTreeType, testNumber, out treeType);

            var initInstructionNode = node["Instructions"];
            Assert.IsNotNull(initInstructionNode, "Couldn't find node 'Instructions'" + TestHelper.AppendTestID(testNumber));
            foreach (XmlNode instruction in initInstructionNode.ChildNodes)
            {
                var method = treeType.GetMethod(instruction.Name);

                Assert.IsNotNull(method, "Couldn't find method: '" + instruction.Name + "' in type: '" + treeType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

                object value = TestHelper.GetObjectValue(instruction, testNumber);
                method.Invoke(binaryTree, new object[] { value });
            }

            var countProperty = treeType.GetProperty("Count");
            Assert.IsNotNull(countProperty, "Couldn't find property: 'Count' on type:" + treeType.ToString() + TestHelper.AppendTestID(testNumber));

            var resultNode = node["Result"];
            Assert.IsNotNull(resultNode, "Couldn't find node 'Result'" + TestHelper.AppendTestID(testNumber));

            int beforeClear = int.Parse(resultNode.Attributes["expectedValueBeforeClear"].Value);
            int afterClear = int.Parse(resultNode.Attributes["expectedValueAfterClear"].Value);

            int count = (int)countProperty.GetValue(binaryTree, null);

            Assert.AreEqual(beforeClear, count, "Before clear" + TestHelper.AppendTestID(testNumber));

            var findMethod = treeType.GetMethod("Clear");
            Assert.IsNotNull(findMethod, "Couldn't find method: 'Clear' on type:" + treeType.ToString() + TestHelper.AppendTestID(testNumber));

            findMethod.Invoke(binaryTree, null);

            count = (int)countProperty.GetValue(binaryTree, null);

            Assert.AreEqual(afterClear, count, "Can still find values after clear" + TestHelper.AppendTestID(testNumber));
        }

        internal static void AddRangeTest(XmlNode node, Type binaryTreeType, int testNumber)
        {
            Type treeType;
            object binaryTree = CreateTreeObject(node, binaryTreeType, testNumber, out treeType);

            Type collectionType;
            object collection = CreateTreeObject(node, typeof(List<>), testNumber, out collectionType);

            var addMethod = collectionType.GetMethod("Add");
            Assert.IsNotNull(addMethod, "Couldn't find method: 'Add' in type: '" + collectionType.ToString() + "'" + TestHelper.AppendTestID(testNumber));
            
            var addValuesNode = node["AddValues"];
            Assert.IsNotNull(addValuesNode, "Couldn't find node 'AddValues'" + TestHelper.AppendTestID(testNumber));
            foreach (XmlNode valueNode in addValuesNode.ChildNodes)
            {
                object value = TestHelper.GetObjectValue(valueNode, testNumber);
                addMethod.Invoke(collection, new object[] { value });
            }

            var addRangeMethod = treeType.GetMethod("InsertRange");
            Assert.IsNotNull(addRangeMethod, "Couldn't find method: 'InsertRange' in type: '" + treeType.ToString() + "'" + TestHelper.AppendTestID(testNumber));

            addRangeMethod.Invoke(binaryTree, new object[] { collection });

            var countProperty = treeType.GetProperty("Count");
            Assert.IsNotNull(countProperty, "Couldn't find property: 'Count' on type:" + treeType.ToString() + TestHelper.AppendTestID(testNumber));

            var resultNode = node["Result"];
            Assert.IsNotNull(resultNode, "Couldn't find node 'Result'" + TestHelper.AppendTestID(testNumber));

            int expectedCount = int.Parse(resultNode.Attributes["value"].Value);
            int count = (int)countProperty.GetValue(binaryTree, null);

            Assert.AreEqual(expectedCount, count, TestHelper.AppendTestID(testNumber));
        
        }
    }
}
