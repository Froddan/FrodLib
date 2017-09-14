using System;
using FrodLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.TestDataClasses;

namespace TestProject
{
    [TestClass]
    public class SuspendNotificationTest
    {
        [TestMethod]
        [TestCategory("NotificationSuspender")]
        public void NotificationSuspendedTest()
        {
            bool notificationSuspened = false;
            int notificationsRunned = 0;
            Person p = new Person();
            p.PropertyChanged += (s, a) => 
            { 
                if(notificationSuspened)
                {
                    Assert.Fail("Notifications shouldn't be throwned at this movement");
                }
                notificationsRunned++;
            };

            p.Age = 21;
            Assert.AreEqual(1, notificationsRunned);
            using(var notifySuspender = new NotificationSuspender(p))
            {
                
                notificationSuspened = true;

                p.Name = "Test";
                p.ShoeSize = 40;
                
                notificationsRunned = 0;
                notificationSuspened = false;
            }

            Assert.AreEqual(2, notificationsRunned);
        }
    }
}
