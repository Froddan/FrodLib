using System;
using System.Globalization;
using FrodLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class TimeTests
    {
        [TestMethod]
        [TestCategory("Time")]
        public void TimeCreationTest()
        {
            CreateTimeTest(24, 0, 0, true);
            CreateTimeTest(23, 60, 30, true);
            CreateTimeTest(23, 0, 670, true);
            CreateTimeTest(-1, 10, 20, true);
            CreateTimeTest(23, -1, 10, true);
            CreateTimeTest(20, 40, -10, true);
            CreateTimeTest(15, 15, 15, false);
        }

        [TestCategory("Time")]
        private void CreateTimeTest(int hour, int minute, int second, bool shouldThrow)
        {
            try
            {
                var t = new Time(hour, minute, second);
                if (shouldThrow)
                {
                    Assert.Fail("Should have throwned exception");
                }
            }
            catch (Exception e)
            {
                if (!shouldThrow)
                {
                    Assert.Fail(string.Format("Caught unwanted exception: {0}", e.Message));
                }
            }
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeStringFormattingTest()
        {
            Time time = new Time(21, 59, 1);
            Assert.AreEqual("21:59:01", time.ToString("HH:mm:ss"));
            Assert.AreEqual("01:21:59", time.ToString("ss:HH:mm"));
            Assert.AreEqual("21-59-01", time.ToString("HH-mm-ss"));
            Assert.AreEqual("01:59:21", time.ToString("ss:mm:HH"));

            Assert.AreEqual("21", time.ToString("HH"));
            Assert.AreEqual("59", time.ToString("mm"));
            Assert.AreEqual("01", time.ToString("ss"));

            Assert.AreEqual("21", time.ToString("H"));
            Assert.AreEqual("59", time.ToString("m"));
            Assert.AreEqual("1", time.ToString("s"));

            string timeS = time.ToString(null);
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeConversionTest()
        {
            Time time = new Time(21, 59, 1);
            TimeSpan timeSpan = time;

            Assert.AreEqual(21, timeSpan.Hours);
            Assert.AreEqual(59, timeSpan.Minutes);
            Assert.AreEqual(1, timeSpan.Seconds);

            Time time2 = (Time)timeSpan;

            Assert.AreEqual(21, time2.Hour);
            Assert.AreEqual(59, time2.Minute);
            Assert.AreEqual(1, time2.Second);
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeEqualityTest()
        {
            Time t1 = new Time(20, 30, 40);
            Time t2 = new Time(15, 45, 40);
            Time t3 = new Time(10, 59, 40);
            Time t4 = t2;

            Assert.IsTrue(t2 == t4);
            Assert.IsTrue(t1 != t2);
            Assert.IsFalse(t2 == t1);
            Assert.IsFalse(t2 != t4);

            Assert.IsTrue(t2 >= t4);
            Assert.IsTrue(t2 >= t3);
            Assert.IsTrue(t2 <= t1);
            Assert.IsTrue(t3 <= t2);

            Assert.IsFalse(t2 >= t1);
            Assert.IsFalse(t2 <= t3);

            Assert.IsTrue(t2 > t3);
            Assert.IsTrue(t2 < t1);

            Assert.IsFalse(t2 > t1);
            Assert.IsFalse(t2 > t4);
            Assert.IsFalse(t2 < t3);
            Assert.IsFalse(t2 < t4);
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeCompareTest()
        {
            Time t1 = new Time(20, 30, 40);
            Time t2 = new Time(15, 45, 40);
            Time t3 = new Time(10, 59, 40);
            Time t4 = t2;

            Time t5 = new Time(15, 46, 40);
            Time t6 = new Time(15, 46, 41);
            Time t7 = new Time(15, 46, 39);

            Assert.IsTrue(t2.CompareTo(t4) == 0);
            Assert.IsTrue(t2.CompareTo(t1) < 0);
            Assert.IsTrue(t2.CompareTo(t3) > 0);
            Assert.IsTrue(t1.CompareTo(t3) > 0);

            Assert.IsTrue(t2.CompareTo(t5) < 0);
            Assert.IsTrue(t5.CompareTo(t6) < 0);
            Assert.IsTrue(t5.CompareTo(t7) > 0);
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeParseTest()
        {
            string[] formats =  { "HH:mm:ss", "H:m:s", "HH:mm", "H:m", "HH-mm-ss", "HH:Mmm", "HH:mm:ss.fff", "HHmmssfff" };

            int testCount = 1;
            try
            {


                Time tExpected = new Time(10, 5, 30);
                Time t2Expected = new Time(10, 5, 0);
                Time t3Expected = new Time(2, 5, 7);
                Time t4Expected = new Time(2, 5, 7, 666);
                Time t1 = Time.ParseExact("10:05:30", formats[0], CultureInfo.CurrentCulture);
                Assert.AreEqual(tExpected, t1, "Test " + (testCount++));

                try
                {
                    Time t2 = Time.ParseExact("10:5:30", formats[1], CultureInfo.CurrentCulture);
                    Assert.Fail("Test " + (testCount++));
                }
                catch (FormatException)
                {
                    testCount++;
                }

                Time t3 = Time.ParseExact("10:05", formats[2], CultureInfo.CurrentCulture);
                Assert.AreEqual(t2Expected, t3, "Test " + (testCount++));

                try
                {
                    Time t4 = Time.ParseExact("10:05", formats[3], CultureInfo.CurrentCulture);
                    Assert.Fail("Test " + (testCount++));
                }
                catch (FormatException)
                {
                    testCount++;
                }

                Time t5 = Time.ParseExact("10-05-30", formats[4], CultureInfo.CurrentCulture);
                Assert.AreEqual(tExpected, t5, "Test " + (testCount++));

                Time t6 = Time.ParseExact("2:5:7", formats[1], CultureInfo.CurrentCulture);
                Assert.AreEqual(t3Expected, t6, "Test " + (testCount++));

                Time t7 = Time.ParseExact("10:M05", formats[5], CultureInfo.CurrentCulture);
                Assert.AreEqual(t2Expected, t7, "Test " + (testCount++));

                Time t8 = Time.ParseExact("02:05:07.666", formats[6], CultureInfo.CurrentCulture);
                Assert.AreEqual(t4Expected, t8, "Test " + (testCount++));

                Time t9 = Time.ParseExact("020507666", formats[7], CultureInfo.CurrentCulture);
                Assert.AreEqual(t4Expected, t9, "Test " + (testCount++));

                try
                {
                    Time t4 = Time.ParseExact("26:00:00", formats[0], CultureInfo.CurrentCulture);
                    Assert.Fail("Test " + (testCount++));
                }
                catch (FormatException)
                {
                    testCount++;
                }

                try
                {
                    Time t4 = Time.ParseExact("00:70:00", formats[0], CultureInfo.CurrentCulture);
                    Assert.Fail("Test " + (testCount++));
                }
                catch (FormatException)
                {
                    testCount++;
                }

                try
                {
                    Time t4 = Time.ParseExact("00:00:70", formats[0], CultureInfo.CurrentCulture);
                    Assert.Fail("Test " + (testCount++));
                }
                catch (FormatException)
                {
                    testCount++;
                }
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception e)
            {
                Assert.Fail("Failed test " + testCount + ". " + e.Message);
            }
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeIntervalTest()
        {
            TimeInterval interval = new TimeInterval(new Time(6, 0, 0), new Time(18, 0, 0));
            Assert.IsFalse(interval.IsInsideInterval(new Time(5, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(6, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(12, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(18, 0, 0)));
            Assert.IsFalse(interval.IsInsideInterval(new Time(19, 0, 0)));

            interval = new TimeInterval(new Time(18, 0, 0), new Time(6, 0, 0));
            Assert.IsFalse(interval.IsInsideInterval(new Time(17, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(18, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(19, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(0, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(5, 0, 0)));
            Assert.IsTrue(interval.IsInsideInterval(new Time(6, 0, 0)));
            Assert.IsFalse(interval.IsInsideInterval(new Time(7, 0, 0)));
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeFromDateTimeTest()
        {
            Time t = new Time(new DateTime(2000, 1, 1, 15, 40, 0));
            Assert.AreEqual(new Time(15, 40, 0), t);

            Time t2 = new Time(new DateTime(2000, 1, 2, 12, 0, 30));
            Assert.AreEqual(new Time(12, 0, 30), t2);
        }

        [TestMethod]
        [TestCategory("Time")]
        public void TimeFromTimespancTest()
        {
            Time t = new Time(new TimeSpan(15, 40, 0));
            Assert.AreEqual(new Time(15, 40, 0), t);

            Time t2 = new Time(new TimeSpan(12, 0, 30));
            Assert.AreEqual(new Time(12, 0, 30), t2);

            Time t3 = new Time(new TimeSpan(2, 6, 45, 15));
            Assert.AreEqual(new Time(6, 45, 15), t3);
        }
    }
}
