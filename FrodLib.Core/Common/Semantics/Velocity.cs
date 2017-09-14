using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Semantics
{
    /// <summary>
    /// Encapsulates a weight.
    /// For conversions, see
    /// http://www.convert-me.com/en/
    /// <para>WORK IN PROGRESS</para>
    /// </summary>
    public class Velocity : SemanticDoubleType<Velocity>
    {
        /// <summary>
        /// Creates an Velocity
        /// </summary>
        /// <param name="value">
        /// Velocity in M/S
        /// </param>
        public Velocity(double value) : base(value) { }

        //TODO implement rest of type
        
        public static Distance operator *(Velocity v, TimeSpan s)
        {
            if (v == null) return null;
            return new Distance(v.Value * s.TotalSeconds);
        }

        public static Acceleration operator /(Velocity v, TimeSpan t)
        {
            return new Acceleration(v.Value / t.TotalSeconds);
        }

    }
}
