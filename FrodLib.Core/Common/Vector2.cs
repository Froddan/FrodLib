﻿using FrodLib.MathUtils;
using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FrodLib
{
    public struct Vector2 : IEquatable<Vector2>
    {
        #region Private Fields

        private static Vector2 zeroVector = new Vector2(0f, 0f);
        private static Vector2 unitVector = new Vector2(1f, 1f);
        private static Vector2 unitXVector = new Vector2(1f, 0f);
        private static Vector2 unitYVector = new Vector2(0f, 1f);

        #endregion Private Fields


        #region Public Fields

        public double X { get; private set; }
        public double Y { get; private set; }

        #endregion Public Fields


        #region Properties

        public static Vector2 Zero
        {
            get { return zeroVector; }
        }

        public static Vector2 One
        {
            get { return unitVector; }
        }

        public static Vector2 UnitX
        {
            get { return unitXVector; }
        }

        public static Vector2 UnitY
        {
            get { return unitYVector; }
        }

        #endregion Properties


        #region Constructors

        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(double value)
        {
            this.X = value;
            this.Y = value;
        }

        #endregion Constructors

        #region Public Methods

        public static Vector2 Add(Vector2 value1, Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, double amount1, double amount2)
        {
            return new Vector2(
                FrodMath.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                FrodMath.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, double amount1, double amount2, out Vector2 result)
        {
            result = new Vector2(
                FrodMath.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
                FrodMath.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
        }

        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, double amount)
        {
            return new Vector2(
                FrodMath.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                FrodMath.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, double amount, out Vector2 result)
        {
            result = new Vector2(
                FrodMath.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                FrodMath.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
        }

        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            return new Vector2(
                FrodMath.Clamp(value1.X, min.X, max.X),
                FrodMath.Clamp(value1.Y, min.Y, max.Y));
        }

        public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            result = new Vector2(
                FrodMath.Clamp(value1.X, min.X, max.X),
                FrodMath.Clamp(value1.Y, min.Y, max.Y));
        }

        public static double Distance(Vector2 value1, Vector2 value2)
        {
            double v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return (double)Math.Sqrt((v1 * v1) + (v2 * v2));
        }

        public static void Distance(ref Vector2 value1, ref Vector2 value2, out double result)
        {
            double v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            result = (double)Math.Sqrt((v1 * v1) + (v2 * v2));
        }

        public static double DistanceSquared(Vector2 value1, Vector2 value2)
        {
            double v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return (v1 * v1) + (v2 * v2);
        }

        public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out double result)
        {
            double v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            result = (v1 * v1) + (v2 * v2);
        }

        public static Vector2 Divide(Vector2 value1, Vector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        public static Vector2 Divide(Vector2 value1, double divider)
        {
            double factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        public static void Divide(ref Vector2 value1, double divider, out Vector2 result)
        {
            double factor = 1 / divider;
            result = new Vector2();
            result.X = value1.X * factor;
            result.Y = value1.Y * factor;
        }

        public static double Dot(Vector2 value1, Vector2 value2)
        {
            return (value1.X * value2.X) + (value1.Y * value2.Y);
        }

        public static void Dot(ref Vector2 value1, ref Vector2 value2, out double result)
        {
            result = (value1.X * value2.X) + (value1.Y * value2.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                return Equals((Vector2)this);
            }

            return false;
        }

        public bool Equals(Vector2 other)
        {
            return FrodMath.Equals(X,other.X) && FrodMath.Equals(Y,other.Y);
        }

        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            Vector2 result;
            double val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
            result = new Vector2();
            result.X = vector.X - (normal.X * val);
            result.Y = vector.Y - (normal.Y * val);
            return result;
        }

        public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
        {
            double val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
            result = new Vector2();
            result.X = vector.X - (normal.X * val);
            result.Y = vector.Y - (normal.Y * val);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, double amount)
        {
            Vector2 result = new Vector2();
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }

        public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, double amount, out Vector2 result)
        {
            result = new Vector2();
            result.X = FrodMath.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
            result.Y = FrodMath.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
        }


        public double Length()
        {
            return (double)Math.Sqrt((X * X) + (Y * Y));
        }

        public double LengthSquared()
        {
            return (X * X) + (Y * Y);
        }

        public static Vector2 Lerp(Vector2 value1, Vector2 value2, double amount)
        {
            return new Vector2(
                FrodMath.Lerp(value1.X, value2.X, amount),
                FrodMath.Lerp(value1.Y, value2.Y, amount));
        }

        public static void Lerp(ref Vector2 value1, ref Vector2 value2, double amount, out Vector2 result)
        {
            result = new Vector2(
                FrodMath.Lerp(value1.X, value2.X, amount),
                FrodMath.Lerp(value1.Y, value2.Y, amount));
        }

        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X > value2.X ? value1.X : value2.X,
                               value1.Y > value2.Y ? value1.Y : value2.Y);
        }

        public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X > value2.X ? value1.X : value2.X;
            result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
        }

        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X < value2.X ? value1.X : value2.X,
                               value1.Y < value2.Y ? value1.Y : value2.Y);
        }

        public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X < value2.X ? value1.X : value2.X;
            result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
        }

        public static Vector2 Multiply(Vector2 value1, Vector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static Vector2 Multiply(Vector2 value1, double scaleFactor)
        {
            value1.X *= scaleFactor;
            value1.Y *= scaleFactor;
            return value1;
        }

        public static void Multiply(ref Vector2 value1, double scaleFactor, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        public static Vector2 Negate(Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            result = new Vector2();
            result.X = -value.X;
            result.Y = -value.Y;
        }

        public void Normalize()
        {
            double val = 1.0f / (double)Math.Sqrt((X * X) + (Y * Y));
            X *= val;
            Y *= val;
        }

        public static Vector2 Normalize(Vector2 value)
        {
            double val = 1.0f / (double)Math.Sqrt((value.X * value.X) + (value.Y * value.Y));
            value.X *= val;
            value.Y *= val;
            return value;
        }

        public static void Normalize(ref Vector2 value, out Vector2 result)
        {
            double val = 1.0f / (double)Math.Sqrt((value.X * value.X) + (value.Y * value.Y));
            result = new Vector2();
            result.X = value.X * val;
            result.Y = value.Y * val;
        }

        public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, double amount)
        {
            return new Vector2(
                FrodMath.SmoothStep(value1.X, value2.X, amount),
                FrodMath.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, double amount, out Vector2 result)
        {
            result = new Vector2(
                FrodMath.SmoothStep(value1.X, value2.X, amount),
                FrodMath.SmoothStep(value1.Y, value2.Y, amount));
        }

        public static Vector2 Subtract(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result = new Vector2();
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[] {
                this.X.ToString(currentCulture), this.Y.ToString(currentCulture) });
        }

        #endregion Public Methods

        #region Operators

        public static Vector2 operator -(Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }


        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return FrodMath.Equals(value1.X,value2.X) && FrodMath.Equals(value1.Y,value2.Y);
        }


        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return !(value1==value2);
        }


        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }


        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }


        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }


        public static Vector2 operator *(Vector2 value, double scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }


        public static Vector2 operator *(double scaleFactor, Vector2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }


        public static Vector2 operator /(Vector2 value1, Vector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }


        public static Vector2 operator /(Vector2 value1, double divider)
        {
            double factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        #endregion Operators
    }
}
