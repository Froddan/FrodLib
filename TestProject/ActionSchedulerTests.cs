using System;
using System.Linq;
using System.Threading;
using FrodLib;
using FrodLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass()]
    public class ActionSchedulerTests
    {

        //[TestMethod()]
        //public void AddScheduledActionTest()
        //{
        //    int calls = 0;
        //    DateTime nowBefore = DateTime.Now;
        //    // Shims can be used only in a ShimsContext:
        //    using (ShimsContext.Create())
        //    {
        //        ManualResetEvent resetEvent = new ManualResetEvent(false);
        //        ManualResetEvent resetEvent2 = new ManualResetEvent(false);

        //        bool runned = false;
        //        bool runned2 = false;

        //        int second = 2;

        //        // Arrange:
        //        // Shim DateTime.Now to return a fixed date:
        //        System.Fakes.ShimDateTime.NowGet = () =>
        //        {
        //            calls++;
        //            return new DateTime(2010, 1, 1, 0, 0, second);
        //        };

        //        ActionScheduler scheduler = new ActionScheduler();
        //        scheduler.AddScheduledAction(() =>
        //        {
        //            second = 5;
        //            runned = true;
        //            resetEvent.Set();
        //        }, new Time(0, 0, 5));

        //        scheduler.AddScheduledAction(() =>
        //        {
        //            runned2 = true;
        //            resetEvent2.Set();
        //        }, new Time(0, 0, 8));

        //        resetEvent.WaitOne(10000);
        //        resetEvent2.WaitOne(10000);

        //        Assert.IsTrue(runned && runned2);
        //    }
        //    DateTime nowAfter = DateTime.Now;
        //    var span = nowAfter - nowBefore;
        //    Assert.AreEqual(9, span.Seconds);
        //}

        //[TestMethod()]
        //public void ClearAllScheduledActionsTest()
        //{

        //    // Shims can be used only in a ShimsContext:
        //    using (ShimsContext.Create())
        //    {
        //        ManualResetEvent resetEvent = new ManualResetEvent(false);
        //        ManualResetEvent resetEvent2 = new ManualResetEvent(false);

        //        bool[] runned = { false, false };

        //        // Arrange:
        //        // Shim DateTime.Now to return a fixed date:
        //        System.Fakes.ShimDateTime.NowGet = () =>
        //        {
        //            return new DateTime(2010, 1, 1, 0, 0, 0);
        //        };

        //        ActionScheduler scheduler = new ActionScheduler();
        //        scheduler.AddScheduledAction(() =>
        //        {
        //            runned[0] = true;
        //            resetEvent.Set();
        //        }, new Time(0, 0, 7));

        //        scheduler.AddScheduledAction(() =>
        //        {
        //            runned[1] = true;
        //            resetEvent2.Set();
        //        }, new Time(0, 0, 6));

        //        Assert.AreEqual(2, scheduler.NumberOfScheduledActions);
        //        Assert.AreEqual(new DateTime(2010, 1, 1, 0, 0, 6), scheduler.NextScheduledActionTime);
        //        scheduler.ClearAllScheduledActions();
        //        Assert.IsNull(scheduler.NextScheduledActionTime);
        //        Assert.AreEqual(0, scheduler.NumberOfScheduledActions);

        //        resetEvent.WaitOne(10000);
        //        resetEvent2.WaitOne(10000);

        //        Assert.IsFalse(runned.Any(b => b));

        //    }
        //}
    }
}
