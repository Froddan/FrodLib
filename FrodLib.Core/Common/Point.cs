using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FrodLib.Extensions;

namespace FrodLib
{

    public struct Point : IEquatable<Point>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "{0};{1}".Format(X, Y);
        }

        public static bool operator ==(Point pointA, Point pointB)
        {
            return pointA.Equals(pointB);
        }

        public static bool operator !=(Point pointA, Point pointB)
        {
            return !pointA.Equals(pointB);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Rect))
            {
                return false;
            }
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 269) ^ this.X.GetHashCode();
                hash = (hash * 269) ^ this.Y.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Point other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
    }
}
