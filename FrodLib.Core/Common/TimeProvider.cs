using FrodLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }

    public abstract class TimeProvider : ITimeProvider
    {
        private static TimeProvider current = DefaultTimeProvider.Instance;

        public static TimeProvider Current
        {
            get { return TimeProvider.current; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                TimeProvider.current = value;
            }
        }

        public abstract DateTime Now { get; }
        public abstract DateTime UtcNow { get; }
        public abstract DateTime Today { get; }

        public static void ResetToDefault()
        {
            TimeProvider.current = DefaultTimeProvider.Instance;
        }
    }

    public class DefaultTimeProvider  : TimeProvider
    {
        public static TimeProvider Instance
        {
            get
            {
                return Singleton<DefaultTimeProvider>.Instance;
            }
        }

        public override DateTime Now
        {
            get
            {
                return Clock.Now;
            }
        }

        public override DateTime Today
        {
            get
            {
                return Clock.Today;
            }
        }

        public override DateTime UtcNow
        {
            get
            {
                return Clock.UtcNow;
            }
        }
    }
}
