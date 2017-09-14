using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FrodLib.Collections
{
    /// <summary>
    /// WORK IN PROGRESS
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class InternalDList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        public static readonly T[] EmptyArray = new T[0];

        private T[] _array;
        private int _count, _start;

        public InternalDList(int capacity)
        {
            _count = _start = 0;
            _array = capacity != 0 ? new T[capacity] : EmptyArray;
        }
        public InternalDList(T[] array, int count)
        {
            _array = array;
            _count = count;
            _start = 0;
        }

        public void PushLast(T item)
        {
            IncreaseCapacity(1);

            int i = NewIndexLast();
            _array[i] = item;
            ++_count;
        }

        private int NewIndexLast()
        {
            int i = _start + _count;
            if (i >= _array.Length)
                i -= _array.Length;
            return i;
        }

        private int InternalIndex(int index)
        {
            int i = _start + index;
            if (i >= _array.Length)
                i -= _array.Length;
            return i;
        }

        private void IncreaseCapacity(int more)
        {
            if (_count + more > _array.Length)
            {
                Capacity = NextLargerSize(_count + more - 1);
            }
        }

        private static int NextLargerSize(int than)
        {
            return ((than << 1) - (than >> 2) + 2) & ~1;
        }

        public void PushLast(ICollection<T> items)
        {
            int count = items.Count;
            IncreaseCapacity(count);

            foreach (T item in items)
            {
                int index = NewIndexLast();
                _array[index] = item;
                ++_count;
            }

        }

        public void PushLast(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                PushLast(item);
            }
        }

        public void PushFirst(T item)
        {
            IncreaseCapacity(1);

            if (_start == 0)
            {
                CopyForward(_array, _start, _start + 1, _count);
                _array[0] = item;
                _count++;
            }
            else
            {
                int lastIndex = NewIndexLast();
                int possibleNewStartIndex = _start - 1;
                if (possibleNewStartIndex != lastIndex)
                {
                    _array[possibleNewStartIndex] = item;
                    _start = possibleNewStartIndex;
                    _count++;
                }
            }
        }

        private void CopyForward(T[] array, int indexFrom, int indexTo, int amount)
        {
            if (amount < 16)
            {
                int stop = indexTo + amount;
                while (indexTo < stop)
                    array[indexTo++] = array[indexFrom++];
            }
            else
            {
                Array.Copy(array, indexFrom - amount, array, indexTo - amount, amount);
            }
        }

        private void CopyBackward(T[] array, int indexFrom, int indexTo, int amount)
        {
            if (amount < 16)
            {
                while (indexTo < _count)
                {
                    if (indexFrom < array.Length)
                    {
                        array[indexTo++] = array[indexFrom++];
                    }
                    else
                    {
                        array[indexTo++] = default(T);
                    }
                }
            }
            else
            {
                Array.Copy(array, indexFrom, array, indexTo, amount);
            }
        }

        public void PushFirst(ICollection<T> items)
        {
            int count = items.Count;
            IncreaseCapacity(count);

            if (_start == 0)
            {
                CopyForward(_array, _start, _start + count, _count);

                int insertIndex = 0;
                foreach (var item in items)
                {
                    _array[insertIndex++] = item;
                    _count++;
                }
            }
            else
            {
                int lastIndex = NewIndexLast();
                int possibleNewStartIndex = _start - count - 1;
                if (possibleNewStartIndex != lastIndex)
                {
                    int tmpPossibleStartIndex = possibleNewStartIndex;
                    foreach (var item in items)
                    {
                        _array[possibleNewStartIndex++] = item;
                        _count++;
                    }
                    _start = tmpPossibleStartIndex;
                }
            }
        }

        public void PushFirst(IEnumerable<T> items)
        {
            foreach (var item in items.Reverse())
            {
                PushFirst(item);
            }
        }

        public void InsertRange(int index, ICollection<T> items)
        {
            if (index == 0)
            {
                PushFirst(items);
            }
            else
            {
                int amount = items.Count;
                IncreaseCapacity(amount);
                CopyForward(_array, index, index + amount, _count - index);
                foreach (var item in items)
                {
                    _array[index++] = item;
                    _count++;
                }
            }
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            if (index == 0)
            {
                PushFirst(items);
            }
            else
            {
                //reverse the collection so it is added in the correct order
                foreach (var item in items.Reverse())
                {
                    Insert(index, item);
                }
            }
        }

        public void AddRange(ICollection<T> items)
        {
            PushLast(items);
        }

        public void AddRange(IEnumerable<T> items)
        {
            PushLast(items);
        }

        public void RemoveRange(ICollection<T> items)
        {

            foreach (var item in items)
            {
                Internal_RemoveItem(item);
            }
            AutoShrink();
        }

        private void Internal_RemoveAt(int index)
        {
            if (index == _start)
            {
                _array[index] = default(T);
                _start++;
            }
            else if (index > _start)
            {
                if (IsDivided)
                {
                    if (index == _array.Length - 1)
                    {
                        _array[index] = _array[0];
                        CopyBackward(_array, 1, 0, _start);
                    }
                    else
                    {
                        CopyBackward(_array, index + 1, index, _array.Length - index - 1);
                        _array[_array.Length - 1] = _array[0];
                        CopyBackward(_array, 1, 0, _start);
                    }
                }
                else
                {
                    CopyBackward(_array, index + 1, index, _array.Length - index - 1);
                }
            }
            else if (index < _start)
            {
                CopyBackward(_array, index + 1, index, _start - index);
            }

            _count--;
        }

        private bool Internal_RemoveItem(T item)
        {
            if (item == null) return false;
            for (int i = 0; i < _array.Length; i++)
            {
                if (item.Equals(_array[i]))
                {
                    Internal_RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        public void RemoveRange(int startIndex)
        {
            RemoveRange(startIndex, -1);
        }

        /// <summary>
        /// Removes elements from starting from the startindex and removes the number of elements specified by count
        /// <para>If count is less then zero then remove all the items starting from start index</para>
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public void RemoveRange(int startIndex, int count)
        {
            if (count < 0)
            {
                count = _count - startIndex;
            }
            if (startIndex == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    _array[i + _start] = default(T);
                    _count--;
                }
                _start += count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int internalIndex = InternalIndex(i + startIndex);
                    _array[internalIndex] = default(T);
                    _count--;
                }
                CopyBackward(_array, InternalIndex(startIndex + count), InternalIndex(startIndex), _count - startIndex);
            }
        }

        public int Capacity
        {
            get
            {
                return _array.Length;
            }
            set
            {
                int delta = value - _array.Length;
                if (delta == 0)
                    return;
                Debug.Assert(value >= _count);
                T[] newArray = new T[value];
                int size1 = FirstHalfSize;
                int size2 = _count - size1;
                Array.Copy(_array, _start, newArray, 0, size1);
                if (size2 > 0)
                    Array.Copy(_array, 0, newArray, size1, size2);
                _start = 0;
                _array = newArray;

            }
        }

        #region IList

        public int FirstHalfSize { get { return Min(_array.Length - _start, _count); } }
        public bool IsDivided { get { return _start + _count > _array.Length; } }

        public int IndexOf(T item)
        {
            for(int i = 0; i<_count ; i++)
            {
                int internalID = InternalIndex(i);
                if (_array[internalID].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if(index == 0)
            {
                PushFirst(item);
            }
            else
            {
                int amount = 1;
                IncreaseCapacity(amount);
                CopyForward(_array, index, index + amount, _count - index);
                _array[index++] = item;
                _count++;
            }
        }

        public void RemoveAt(int index)
        {
            Internal_RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                int internalIndex = InternalIndex(index);
                return _array[internalIndex];
            }
            set
            {
                int internalIndex = InternalIndex(index);
                _array[internalIndex] = value;
            }
        }

        public void Add(T item)
        {
            PushLast(item);
        }

        public void Clear()
        {
            _array = EmptyArray;
            _count = 0;
            _start = 0;
        }

        public bool Contains(T item)
        {
            return _array.Contains(item);
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            IEnumerator<T> enumerator = this.GetEnumerator();

            for (int i = arrayIndex; i < array.Length; i++)
            {
                if (enumerator.MoveNext())
                    array[i] = enumerator.Current;
                else
                    break;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            bool result = Internal_RemoveItem(item);
            AutoShrink();

            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i< _array.Length; i++)
            {
                yield return _array[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IList

        static int Min(int x, int y)
        {
            return x + (((y - x) >> 31) & (y - x));
        }

        private void AutoShrink()
        {
            if ((_count << 1) + 2 < _array.Length)
                Capacity = _count + 2;
        }
    }
}
