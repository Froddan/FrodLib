using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrodLib.Collections;

namespace TestProject.CollectionTests
{
    [TestClass]
    internal class DListTests : IListExtendedTestsImpl<DList<object>>
    {

        protected override DList<object> CreateInstance()
        {
            return new DList<object>();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void AddRemoveInsertRangeTest()
        {

            base.AddRemoveInsertRangeTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void AddRemoveInsertItemTest()
        {
            base.AddRemoveInsertItemTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void ClearTest()
        {
            base.ClearTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void ContainsTest()
        {
            base.ContainsTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void CopyToArrayTest()
        {
            base.CopyToArrayTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void RandomAccessTest()
        {
            base.RandomAccessTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public override void SetCapacityTest()
        {
            base.SetCapacityTest();
        }

        [TestMethod]
        [TestCategory("DList")]
        public void PushFirstLastTest()
        {
            throw new NotImplementedException();
        }
    }
}
