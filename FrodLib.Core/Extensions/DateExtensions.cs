using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace FrodLib.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToStartOfMonth(this DateTime datetime)
        {
            if (datetime.Day == 1)
            {
                return datetime;
            }
            else
            {
                return new DateTime(datetime.Year, datetime.Month, 1);
            }
        }

        public static string GetMonthName(this DateTime datetime, CultureInfo culture = null)
        {
            DateTimeFormatInfo datetimeFormatInfo;
            if(culture == null)
            {
                datetimeFormatInfo = new DateTimeFormatInfo();
            }
            else
            {
                datetimeFormatInfo = culture.DateTimeFormat;
            }
            return datetimeFormatInfo.GetMonthName(datetime.Month);
        }

        public static string GetMonthShortName(this DateTime datetime, CultureInfo culture = null)
        {
            string monthName = GetMonthName(datetime, culture);
            return monthName.Substring(0, Math.Min(monthName.Length, 3));
        }

        public static string GetDayOfWeekName(this DateTime datetime, CultureInfo culture = null)
        {
            DateTimeFormatInfo datetimeFormatInfo;
            if (culture == null)
            {
                datetimeFormatInfo = new DateTimeFormatInfo();
            }
            else
            {
                datetimeFormatInfo = culture.DateTimeFormat;
            }
            return datetimeFormatInfo.GetDayName(datetime.DayOfWeek);
        }

        public static string GetDayOfWeekShortName(this DateTime datetime, CultureInfo culture = null)
        {
            string dayName = GetDayOfWeekName(datetime, culture);
            return dayName.Substring(0, Math.Min(dayName.Length, 3));
        }

        public static TimeSpan Hours(this int hours)
        {
            return new TimeSpan(hours, 0, 0);
        }

        public static TimeSpan Minutes(this int minutes)
        {
            return new TimeSpan(0, minutes, 0);
        }

        public static TimeSpan Seconds(this int seconds)
        {
            return new TimeSpan(0, 0, seconds);
        }

        public static TimeSpan Days(this int days)
        {
            return new TimeSpan(days,0, 0, 0);
        }
    }
}
