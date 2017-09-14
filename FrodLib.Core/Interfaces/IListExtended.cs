using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib
{
    public interface IListExtended<T> : IList<T>
    {
        void InsertRange(int index, ICollection<T> collection);

        void InsertRange(int index, IEnumerable<T> items);

        void AddRange(ICollection<T> collection);

        void AddRange(IEnumerable<T> items);

        void RemoveRange(ICollection<T> collection);

        void RemoveRange(IEnumerable<T> items);

        void RemoveRange(int startIndex, int count);

        T this[int index, T defaultValue]
        {
            get;
        }

        int Capacity { get; set; }
    }
}
