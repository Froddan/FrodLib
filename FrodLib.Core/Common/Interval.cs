using FrodLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    public class Interval<T> : IInterval<T> where T : IComparable<T>
    {
        public T StartInterval { get; private set; }
        public T EndInterval { get; private set; }

        public Interval(T start, T end)
        {
            if (start.CompareTo(end) > 0)
            {
                StartInterval = end;
                EndInterval = start;
            }
            else
            {
                StartInterval = start;
                EndInterval = end;
            }
        }

        public bool IsOverlapping(Interval<T> other)
        {
            if (this.EndInterval.CompareTo(other.StartInterval) < 0)
            {
                return false;
            }
            else if (this.StartInterval.CompareTo(other.EndInterval) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0} - {1}]", StartInterval, EndInterval);
        }

        public Interval<T> Intersection(Interval<T> other)
        {
            if (!this.IsOverlapping(other))
            {
                throw new InvalidOperationException("Intervals are not intersecting");
            }

            T start = Max(this.StartInterval, other.StartInterval);
            T end = Min(this.EndInterval, other.EndInterval);

            return new Interval<T>(start, end);
        }

        private T Max(T v1, T v2)
        {
            if (v1.CompareTo(v2) >= 0) return v1;
            else return v2;
        }

        private T Min(T v1, T v2)
        {
            if (v1.CompareTo(v2) <= 0) return v1;
            else return v2;
        }

        public bool IsInsideInterval(T value)
        {
            if (this.EndInterval.CompareTo(value) > 0)
            {
                return false;
            }
            else if (this.StartInterval.CompareTo(value) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
