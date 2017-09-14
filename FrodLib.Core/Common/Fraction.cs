using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    public struct Fraction : IEquatable<Fraction>, IEquatable<double>, IComparable<Fraction>, IComparable<double>
    {
        public int Numerator { get; private set; }
        public int Denominator { get; private set; }
        public static readonly Fraction Zero = new Fraction(0, 0);

        public Fraction(int numerator, int denominator)
            : this()
        {
            this.Numerator = numerator;
            this.Denominator = denominator;

            if (Denominator < 0)
            {
                this.Numerator = -this.Numerator;
                this.Denominator = -this.Denominator;
            }
        }

        public Fraction(int numerator, Fraction denominator)
        {
            this = new Fraction(numerator, 1) / denominator;
        }

        public Fraction(Fraction numerator, int denominator)
        {
            this = numerator * new Fraction(1, denominator);
        }

        public Fraction(Fraction fraction)
            : this()
        {
            this.Numerator = fraction.Numerator;
            this.Denominator = fraction.Denominator;
        }

        public Fraction(double value)
        {
            double orgValue = value;
            try
            {
                int denominator = 1;
                while (value % 1 != 0)
                {
                    value *= 10;
                    denominator *= 10;
                }

                this = new Fraction((int)value, denominator).GetReduced();
            }
            catch (OverflowException)
            {
                this = ApproximateFraction(orgValue, 0.00000001d).GetReduced();
            }
            
        }

        private static int GetGCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (b > 0)
            {
                int rem = a % b;
                a = b;
                b = rem;
            }
            return a;
        }

        private static int GetLCD(int a, int b)
        {
            return (a * b) / GetGCD(a, b);
        }

        public Fraction ToDenominator(int targetDenominator)
        {
            Fraction modifiedFraction = this;

            if (targetDenominator < this.Denominator)
            {
                return modifiedFraction;
            }

            if (targetDenominator % this.Denominator != 0)
            {
                return modifiedFraction;
            }

            if (this.Denominator != targetDenominator)
            {
                int factor = targetDenominator / this.Denominator;
                modifiedFraction.Denominator = targetDenominator;
                modifiedFraction.Numerator *= factor;
            }

            return modifiedFraction;
        }

        public Fraction GetReduced()
        {
            Fraction modifiedFraction = this;
            int gcd = 0;
            while (Math.Abs(gcd = GetGCD(modifiedFraction.Numerator, modifiedFraction.Denominator)) > 1)
            {
                modifiedFraction.Numerator /= gcd;
                modifiedFraction.Denominator /= gcd;
            }

            if (modifiedFraction.Denominator < 0)
            {
                modifiedFraction.Numerator = -modifiedFraction.Numerator;
                modifiedFraction.Denominator = -modifiedFraction.Denominator;
            }

            return modifiedFraction;
        }

        /// <summary>
        /// Returns the Multiplicative inverse of the fraction
        /// </summary>
        /// <returns></returns>
        public Fraction GetReciprocal()
        {
            return new Fraction(this.Denominator, this.Numerator);
        }

        public static Fraction operator +(Fraction fraction1, Fraction fraction2)
        {
            if (fraction1.Denominator == 0)
            {
                return fraction2;
            }
            else if (fraction2.Denominator == 0)
            {
                return fraction1;
            }

            int lcd = GetLCD(fraction1.Denominator, fraction2.Denominator);

            fraction1 = fraction1.ToDenominator(lcd);
            fraction2 = fraction2.ToDenominator(lcd);

            return new Fraction(fraction1.Numerator + fraction2.Numerator, lcd).GetReduced();
        }

        public static Fraction operator -(Fraction fraction1, Fraction fraction2)
        {
            int lcd = GetLCD(fraction1.Denominator, fraction2.Denominator);

            fraction1 = fraction1.ToDenominator(lcd);
            fraction2 = fraction2.ToDenominator(lcd);

            return new Fraction(fraction1.Numerator - fraction2.Numerator, lcd).GetReduced();
        }

        public static Fraction operator *(Fraction fraction1, Fraction fraction2)
        {
            int numerator = fraction1.Numerator * fraction2.Numerator;
            int denominator = fraction1.Denominator * fraction2.Denominator;

            return new Fraction(numerator, denominator);
        }

        public static Fraction operator /(Fraction fraction1, Fraction fraction2)
        {
            if(fraction2 == 0)
            {
                throw new ArithmeticException(StringResources.DivideByZero);
            }
            return new Fraction(fraction1 * fraction2.GetReciprocal()).GetReduced();
        }

        public static bool operator ==(Fraction fraction1, Fraction fraction2)
        {
            return fraction1.Equals(fraction2);
        }

        public static bool operator !=(Fraction fraction1, Fraction fraction2)
        {

            return !fraction1.Equals(fraction2);
        }

        public static bool operator <=(Fraction fraction1, Fraction fraction2)
        {
            return fraction1.ToDouble() <= fraction2.ToDouble();
        }

        public static bool operator >=(Fraction fraction1, Fraction fraction2)
        {

            return fraction1.ToDouble() >= fraction2.ToDouble();
        }

        public static bool operator <(Fraction fraction1, Fraction fraction2)
        {
            return fraction1.ToDouble() < fraction2.ToDouble();
        }

        public static bool operator >(Fraction fraction1, Fraction fraction2)
        {

            return fraction1.ToDouble() > fraction2.ToDouble();
        }

        public double ToDouble()
        {
            if (Numerator == 0)
            {
                return 0;
            }
            else if (Denominator == 0)
            {
                return double.NaN;
            }
            else
            {
                return (double)this.Numerator / this.Denominator;
            }
        }

        /// <summary>
        /// Returns this fraction expressed as a double, rounded to the specified number of decimal places.
        /// Returns double.NaN if denominator is zero
        /// </summary>
        public double ToDouble(int decimals)
        {
            if (Numerator == 0)
            {
                return 0;
            }
            else if (Denominator == 0)
            {
                return double.NaN;
            }
            return Math.Round(ToDouble(), decimals);
        }

        public override string ToString()
        {
            if (Denominator == 0) return Numerator + "/" + Denominator;
            int value = Numerator / Denominator;
            if (Denominator != 0 && value == ToDouble())
            {
                return "" + value;
            }
            else
            {
                return Numerator + "/" + Denominator;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is double)
            {
                return Equals((double)obj);
            }
            else if (obj is Fraction)
            {
                return Equals((Fraction)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 11;
                hash = (hash * 269) ^ this.Numerator.GetHashCode();
                hash = (hash * 269) ^ this.Denominator.GetHashCode();
                return hash;
            }
        }

        public static implicit operator double(Fraction fraction)
        {
            return fraction.ToDouble();
        }

        public static explicit operator float(Fraction fraction)
        {
            return (float)fraction.ToDouble();
        }

        public static implicit operator Fraction(double d)
        {
            return new Fraction(d);
        }

        /// <summary>
        /// Approximates the provided value to a fraction.
        /// </summary>
        /// <param name="value">The double being approximated as a fraction.</param>
        /// <param name="precision">Maximum difference targeted for the fraction to be considered equal to the value.</param>
        /// <returns>Fraction struct representing the value.</returns>
        private static Fraction ApproximateFraction(double value, double precision)
        {
            int numerator = 1;
            int denominator = 1;
            double fraction = numerator / denominator;

            while (System.Math.Abs(fraction - value) > precision)
            {
                if (fraction < value)
                {
                    numerator++;
                }
                else
                {
                    denominator++;
                    numerator = (int)System.Math.Round(value * denominator);
                }

                fraction = numerator / (double)denominator;
            }
            return new Fraction(numerator, denominator);
        }

        public bool Equals(Fraction other)
        {
            return Math.Abs(this.ToDouble() - other.ToDouble()) < double.Epsilon; //numeratorEqual && denominatorEqual;
        }

        public bool Equals(double other)
        {
            return Math.Abs(this.ToDouble() - other) < double.Epsilon;
        }

        public int CompareTo(Fraction other)
        {
            return this.ToDouble().CompareTo(other.ToDouble());
        }

        public int CompareTo(double other)
        {
            return this.ToDouble().CompareTo(other);
        }
    }
}
