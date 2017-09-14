using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FrodLib.Extensions;
using FrodLib.Resources;
using FrodLib.Utils;

namespace FrodLib
{
    public struct Time : IEquatable<Time>, IEquatable<TimeSpan>, IComparable<Time>, IFormattable
    {
        public static readonly Time Zero = new Time(0, 0, 0);
        private readonly byte m_hour;
        private readonly byte m_minute;
        private readonly byte m_second;
        private readonly ushort m_millisecond;

        public Time(int hour, int minute, int second)
            : this()
        {
            if (hour > 23 || hour < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), StringResources.HourOutOfRange);
            }
            if (minute > 59 || minute < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), StringResources.MinutesOutOfRange);
            }
            if (second > 59 || second < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(second), StringResources.SecondsOutOfRange);
            }

            this.m_hour = (byte)hour;
            this.m_minute = (byte)minute;
            this.m_second = (byte)second;
        }

        public Time(int hour, int minute, int second, int millisecond)
            : this()
        {
            if (hour > 23 || hour < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), StringResources.HourOutOfRange);
            }
            if (minute > 59 || minute < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), StringResources.MinutesOutOfRange);
            }
            if (second > 59 || second < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(second), StringResources.SecondsOutOfRange);
            }
            if (millisecond > 999 || millisecond < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecond), StringResources.MilliSecondsOutOfRange);
            }

            this.m_hour = (byte)hour;
            this.m_minute = (byte)minute;
            this.m_second = (byte)second;
            this.m_millisecond = (ushort)millisecond;
        }

        public Time(byte hour, byte minute, byte second)
            : this(hour, minute, second, (ushort)0)
        {

        }

        public Time(byte hour, byte minute, byte second, ushort millisecond)
            : this()
        {
            if (hour > 23 || hour < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), StringResources.HourOutOfRange);
            }
            if (minute > 59 || minute < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), StringResources.MinutesOutOfRange);
            }
            if (second > 59 || second < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(second), StringResources.SecondsOutOfRange);
            }
            if (millisecond > 999 || millisecond < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecond), StringResources.MilliSecondsOutOfRange);
            }

            this.m_hour = hour;
            this.m_minute = minute;
            this.m_second = second;
            this.m_millisecond = millisecond;
        }

        public Time(TimeSpan timespan)
            : this()
        {
            this.m_hour = (byte)timespan.Hours;
            this.m_minute = (byte)timespan.Minutes;
            this.m_second = (byte)timespan.Seconds;
            this.m_millisecond = (byte)timespan.Milliseconds;
        }

        public Time(DateTime dt)
            : this()
        {
            this.m_hour = (byte)dt.Hour;
            this.m_minute = (byte)dt.Minute;
            this.m_second = (byte)dt.Second;
            this.m_millisecond = (byte)dt.Millisecond;
        }

        public static Time Now
        {
            get
            {
                return new Time(Clock.Now);
            }
        }

        public int Hour { get { return m_hour; } }

        public int Minute { get { return m_minute; } }

        public int Second { get { return m_second; } }

        public int MilliSecond { get { return m_millisecond; } }

        public static explicit operator Time(TimeSpan timespan)
        {
            return new Time(timespan);
        }

        public static explicit operator Time(DateTime dt)
        {
            return new Time(dt);
        }

        public static implicit operator TimeSpan(Time time)
        {
            return new TimeSpan(0, time.m_hour, time.m_minute, time.m_second, time.m_millisecond);
        }

        public static bool operator !=(Time time1, Time time2)
        {
            return !time1.Equals(time2);
        }

        public static bool operator <(Time time1, Time time2)
        {
            return time1.CompareTo(time2) < 0;
        }

        public static bool operator <=(Time time1, Time time2)
        {
            return time1.CompareTo(time2) <= 0;
        }

        public static bool operator ==(Time time1, Time time2)
        {
            return time1.Equals(time2);
        }

        public static bool operator >(Time time1, Time time2)
        {
            return time1.CompareTo(time2) > 0;
        }

        public static bool operator >=(Time time1, Time time2)
        {
            return time1.CompareTo(time2) >= 0;
        }

        public static Time Parse(string s)
        {
            return ParseExact(s, new[] { "HH:mm:ss", "H:m:s", "HH:mm", "H:m" }, CultureInfo.CurrentCulture);
        }

        public static Time Parse(string s, IFormatProvider formatProvider)
        {
            return ParseExact(s, new[] { "HH:mm:ss.fff", "HH:mm:ss", "H:m:s", "HH:mm", "H:m" }, formatProvider);
        }

        public static Time ParseExact(string s, string format, IFormatProvider formatProvider)
        {
            return ParseExact(s, new[] { format }, formatProvider);
        }

        public static Time ParseExact(string s, string[] formats, IFormatProvider formatProvider)
        {
            ArgumentValidator.IsNotNull(s, nameof(s));
            ArgumentValidator.IsNotNull(formats, nameof(formats));

            if (formats.Length == 0)
            {
                throw new ArgumentException(StringResources.NoFormatsSpecified);
            }

            foreach (string format in formats)
            {
                try
                {

                    byte hour;
                    byte minute;
                    byte second;
                    ushort millisecond;

                    if (TryParseValue(s, format, 'H', 23, out hour) == false) continue;
                    if (TryParseValue(s, format, 'm', 60, out minute) == false) continue;
                    if (TryParseValue(s, format, 's', 60, out second) == false) continue;
                    if (TryParseValueShort(s, format, 'f', 999, out millisecond) == false) continue;
                    return new Time(hour, minute, second, millisecond);
                }
                catch (FormatException)
                { }
                catch (ArgumentOutOfRangeException)
                { }
            }

            throw new FormatException(StringResources.CannotParseTimeWithFormats);
        }

        public int CompareTo(Time other)
        {
            int compareResult;
            if ((compareResult = this.m_hour.CompareTo(other.m_hour)) != 0) return compareResult;
            else if ((compareResult = this.m_minute.CompareTo(other.m_minute)) != 0) return compareResult;
            else if ((compareResult = this.m_second.CompareTo(other.m_second)) != 0) return compareResult;
            else return this.m_millisecond.CompareTo(other.m_millisecond);
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            else if (object.ReferenceEquals(this, other)) return true;
            else if (other is Time)
            {
                return this.Equals((Time)other);
            }
            else if (other is TimeSpan)
            {
                return this.Equals((TimeSpan)other);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Time other)
        {
            return this.m_hour == other.m_hour && this.m_minute == other.m_minute && this.m_second == other.m_second && this.m_millisecond == other.m_millisecond;
        }

        public bool Equals(TimeSpan other)
        {
            return other.Days == 0 && this.Hour == other.Hours && this.Minute == other.Minutes && this.Second == other.Seconds && this.m_millisecond == other.Milliseconds;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 41;
                hash = (hash * 269) ^ this.m_hour.GetHashCode();
                hash = (hash * 269) ^ this.m_minute.GetHashCode();
                hash = (hash * 269) ^ this.m_second.GetHashCode();
                hash = (hash * 269) ^ this.m_millisecond.GetHashCode();
                return hash;
            }
        }

        internal int TotalSeconds
        {
            get
            {
                return m_hour * 60 * 60 + m_minute * 60 + m_second;
            }
        }

        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return this.ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

            int hoursFormat = format.Count(c => c == 'H');
            int minutesFormat = format.Count(c => c == 'm');
            int secondsFormat = format.Count(c => c == 's');
            int milliSecondsFormat = format.Count(c => c == 'f');

            while (format.Contains("HH") || format.Contains("mm") || format.Contains("ss") || format.Contains("ff") || format.Contains("fff") || format.Contains("tt"))
            {
                format = format.Replace("HH", "H");
                format = format.Replace("mm", "m");
                format = format.Replace("ss", "s");
                format = format.Replace("fff", "f");
                format = format.Replace("ff", "f");
                format = format.Replace("tt", "t");
            }

            List<string> values = new List<string>();
            int index = 0;

            var hour = m_hour;
            if (format.Contains("t"))
            {
                format = format.Replace("t", "{" + index + "}");

                if (hour < 12)
                {
                    values.Add("AM");
                }
                else
                {
                    hour -= 12;
                    values.Add("PM");
                }

                index++;
            }
            if (format.Contains("H"))
            {
                format = format.Replace("H", "{" + index + "}");
               
                values.Add(hour.ToString(new string(Enumerable.Range(0, hoursFormat).Select(s => '0').ToArray())));
                index++;
            }
            if (format.Contains("m"))
            {
                format = format.Replace("m", "{" + index + "}");
                values.Add(m_minute.ToString(new string(Enumerable.Range(0, minutesFormat).Select(s => '0').ToArray())));
                index++;
            }
            if (format.Contains("s"))
            {
                format = format.Replace("s", "{" + index + "}");
                values.Add(m_second.ToString(new string(Enumerable.Range(0, secondsFormat).Select(s => '0').ToArray())));
            }
            if (format.Contains("f"))
            {
                format = format.Replace("f", "{" + index + "}");
                values.Add(m_millisecond.ToString(new string(Enumerable.Range(0, milliSecondsFormat).Select(s => '0').ToArray())));
            }

            return string.Format(format, values.ToArray());
        }

        private static bool TryParseValue(string s, string format, char identifier, byte maxAllowedValue, out byte value)
        {
            int count = format.Count(c => c == identifier);
            int index = format.IndexOf(identifier);
            if (index > -1 && count > 0)
            {
                string str = s.Substring(index, count);
                if (byte.TryParse(str, out value))
                {
                    if (value > maxAllowedValue)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            value = 0;
            return true;
        }

        private static bool TryParseValueShort(string s, string format, char identifier, ushort maxAllowedValue, out ushort value)
        {
            int count = format.Count(c => c == identifier);
            int index = format.IndexOf(identifier);
            if (index > -1 && count > 0)
            {
                string str = s.Substring(index, count);
                if (ushort.TryParse(str, out value))
                {
                    if (value > maxAllowedValue)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            value = 0;
            return true;
        }
    }
}