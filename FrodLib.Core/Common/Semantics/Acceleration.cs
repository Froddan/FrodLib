using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Semantics
{
    /// <summary>
    /// Encapsulates an acceleration.
    /// </summary>
    public class Acceleration : SemanticDoubleType<Acceleration>
    {
        /// <summary>
        /// Creates an Velocity
        /// </summary>
        /// <param name="value">
        /// Velocity in M/S^2
        /// </param>
        public Acceleration(double value) : base(value) { }

        public static Velocity operator *(Acceleration a, TimeSpan t)
        {
            return new Velocity(a.Value * t.TotalSeconds);
        }


        public Velocity CalculateVelocity(TimeSpan time)
        {
            return this * time;
        }

        public Velocity CalculateVelocity(TimeSpan time, Velocity initialVelocity)
        {
            return new Velocity((this.Value * time.TotalSeconds) + initialVelocity.Value);
        }
    }
}
