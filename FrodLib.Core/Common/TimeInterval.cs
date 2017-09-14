using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib
{
    public class TimeInterval : IInterval<Time>
    {
        public Time StartInterval { get; private set; }
        public Time EndInterval { get; private set; }

        public TimeInterval(Time start, Time end)
        {
            StartInterval = start;
            EndInterval = end;
        }

        public TimeInterval(TimeSpan start, TimeSpan end)
        {
            StartInterval = new Time(start);
            EndInterval = new Time(end);
        }

        public TimeInterval(DateTime start, DateTime end)
        {
            StartInterval = new Time(start);
            EndInterval = new Time(end);
        }

        public override string ToString()
        {
            return string.Format("[{0} - {1}]", StartInterval, EndInterval);
        }

        public bool IsInsideInterval(Time time)
        {
            if(StartInterval <= EndInterval)
            {
                return StartInterval <= time && time <= EndInterval;
            }
            else
            {
                return StartInterval <= time || time <= EndInterval;
            }
        }

        public bool IsInsideInterval(DateTime time)
        {
            return IsInsideInterval((Time)time);
        }

        public bool IsInsideInterval(TimeSpan timeOfDay)
        {
            return IsInsideInterval((Time)timeOfDay);
        }
    }
}
