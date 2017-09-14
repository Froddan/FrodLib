using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrodLib.Interfaces
{
    public interface IInterval<TInterval> where TInterval : IComparable<TInterval>
    {
        TInterval StartInterval { get; }
        TInterval EndInterval { get; }

        bool IsInsideInterval(TInterval time);
    }
}
