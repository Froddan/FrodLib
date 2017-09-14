using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    /// <summary>
    /// A static class that gives the same public static properties as DateTime, but are overrideable by changing the properties backing delegates
    /// </summary>
    public static class Clock
    {
       
        /// <summary>
        /// Overrideable time delegate to set an other time then the default "DateTime.Now"
        /// <para>The value from this delegate can be accessed by Clock.Now</para>
        /// </summary>
        public static Func<DateTime> NowDelegate = () => DateTime.Now;

        /// <summary>
        /// Overrideable time delegate to set an other time then the default "DateTime.Today"
        /// <para>The value from this delegate can be accessed by Clock.Today</para>
        /// </summary>
        public static Func<DateTime> TodayDelegate = () => DateTime.Today;

        /// <summary>
        /// Overrideable time delegate to set an other time then the default "DateTime.UtcNow"
        /// <para>The value from this delegate can be accessed by Clock.UtcNow</para>
        /// </summary>
        public static Func<DateTime> UtcNowDelegate = () => DateTime.UtcNow;

        /// <summary>
        /// Returns the current local time
        /// </summary>
        public static DateTime Now
        {
            get
            {
                var timeDelegate = NowDelegate;
                if (timeDelegate == null) return DateTime.Now;
                return timeDelegate();
            }
        }

        /// <summary>
        /// Returns current day
        /// </summary>
        public static DateTime Today
        {
            get
            {
                var timeDelegate = TodayDelegate;
                if (timeDelegate == null) return DateTime.Today;
                return timeDelegate();
            }
        }

        /// <summary>
        /// Returns current UTC time
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                var timeDelegate = UtcNowDelegate;
                if (timeDelegate == null) return DateTime.UtcNow;
                return timeDelegate();
            }
        }
    }
}
