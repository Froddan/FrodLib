using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.MathUtils
{

    public class FrodMath
    {
        public const double E = Math.E;
        public const double Log10E = 0.4342945f;
        public const double Log2E = 1.442695f;
        public const double Pi = Math.PI;
        public const double PiOver2 = (Math.PI / 2.0);
        public const double PiOver4 = (Math.PI / 4.0);
        public const double TwoPi = (Math.PI * 2.0);

        private static readonly int[] _lookup =
        {
            32, 0, 1, 26, 2, 23, 27, 0, 3, 16, 24, 30, 28, 11, 0, 13, 4, 7, 17,
            0, 25, 22, 31, 15, 29, 10, 12, 6, 0, 21, 14, 9, 5, 20, 8, 19, 18
        };

        private static readonly long goodMask;

        static FrodMath()
        {
            long mask = 0;
            for (int i = 0; i < 64; ++i)
            {
                mask |= long.MinValue >> (i * i);
            }
            goodMask = mask;
        }

        // 0xC840C04048404040 computed below



        public static double Barycentric(double value1, double value2, double value3, double amount1, double amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        public static double CatmullRom(double value1, double value2, double value3, double value4, double amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precission
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (double)(0.5 * (2.0 * value2 +
                (value3 - value1) * amount +
                (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }

        public static double Clamp(double value, double min, double max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }

        public static int ConvertRange(
                                    int originalStart, int originalEnd, // original range
            int newStart, int newEnd, // desired range
            int value) // value to convert
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (int)(newStart + ((value - originalStart) * scale));
        }

        public static double ConvertRange(
            double originalStart, double originalEnd, // original range
            double newStart, double newEnd, // desired range
            double value) // value to convert
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (double)(newStart + ((value - originalStart) * scale));
        }

        public static double Distance(double value1, double value2)
        {
            return Math.Abs(value1 - value2);
        }

        /// <summary>
        /// Compares two values and checks if they are "equal" within a specific tolerance level
        /// </summary>
        /// <param name="v1">The first value to compare</param>
        /// <param name="v2">The first value to compare</param>
        /// <param name="tolerance">The tolerance level, default is Epsilon. 
        ///If the value is set lower then epsilon then the value will be set to epsilon</param>
        /// <returns>True if both values are considered equal by the tolerace level</returns>
        public static bool Equals(double v1, double v2, double tolerance = double.Epsilon)
        {
            if (tolerance < double.Epsilon)
                tolerance = double.Epsilon;
            return Math.Abs(v1 - v2) < tolerance;
        }

        /// <summary>
        /// Compares two values and checks if they are "equal" within a specific tolerance level
        /// </summary>
        /// <param name="v1">The first value to compare</param>
        /// <param name="v2">The first value to compare</param>
        /// <param name="tolerance">The tolerance level, default is Epsilon.
        /// If the value is set lower then epsilon then the value will be set to epsilon</param>
        /// <returns>True if both values are considered equal by the tolerace level</returns>
        public static bool Equals(float v1, float v2, float tolerance = float.Epsilon)
        {
            if (tolerance < float.Epsilon)
                tolerance = float.Epsilon;
            return Math.Abs(v1 - v2) < tolerance;
        }

        public static double Hermite(double value1, double tangent1, double value2, double tangent2, double amount)
        {
            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            double sCubed = s * s * s;
            double sSquared = s * s;

            if (Equals(amount, 0d))
                result = value1;
            else if (Equals(amount, 1d))
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                    (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                    t1 * s +
                    v1;
            return (double)result;
        }

        public static bool IsPerfectSquare(long n)
        {
            // This tests if the 6 least significant bits are right.
            // Moving the to be tested bit to the highest position saves us masking.
            if ((goodMask << (int)n) >= 0) return false;
            int numberOfTrailingZeros = NumberOfTrailingZeroes(n);

            // Each square ends with an even number of zeros.
            if ((numberOfTrailingZeros & 1) != 0) return false;
            n >>= numberOfTrailingZeros;

            // Now x is either 0 or odd.
            // In binary each odd square ends with 001.
            // Postpone the sign test until now; handle zero in the branch.
            if ((n & 7) != 1 | n <= 0) return n == 0;

            // Do it in the classical way.
            // The correctness is not trivial as the conversion from long to double is lossy!
            long tst = (long)Math.Sqrt(n);
            return tst * tst == n;
        }

        public static bool IsPowerOfTwo(int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }

        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static double Max(double value1, double value2)
        {
            return Math.Max(value1, value2);
        }

        public static double Min(double value1, double value2)
        {
            return Math.Min(value1, value2);
        }

        public static int NumberOfTrailingZeroes(long n)
        {
            return _lookup[(n & -n) % 37];
        }

        public static double SmoothStep(double value1, double value2, double amount)
        {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            double result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }

        public static double ToDegrees(double radians)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = 180 / pi
            return (double)(radians * 57.295779513082320876798154814105);
        }

        public static double ToRadians(double degrees)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = pi / 180
            return (double)(degrees * 0.017453292519943295769236907684886);
        }

        public static double WrapAngle(double angle)
        {
            angle = (double)Math.IEEERemainder((double)angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }
    }
}
