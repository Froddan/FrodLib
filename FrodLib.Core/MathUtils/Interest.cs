using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.MathUtils
{
    public static class Interest
    {
        public static double CompoundInterest(double principal, double interestRate, int timesPerYear, double years)
        {
            double body = 1 + (interestRate / timesPerYear);
            double exponent = timesPerYear * years;

            return principal * Math.Pow(body, exponent);
        }
    }
}
