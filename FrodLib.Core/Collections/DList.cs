using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Collections
{
    /// <summary>
    /// WORK IN PROGRESS
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DList<T> : IDeque<T>, IListExtended<T>, ICollection<T>, IEnumerable<T>, IList
    {
        private InternalDList<T> _dlist;

        public DList()
        {
            _dlist = new InternalDList<T>(5);
        }

        public DList(int capacity)
        {
            _dlist = new InternalDList<T>(capacity);
        }

        public DList(IEnumerable<T> collection)
        {
            int initialSize = collection.Count();
            _dlist = new InternalDList<T>(initialSize);
            _dlist.PushLast(collection);
        }

        #region IList<T>
        public int IndexOf(T item)
        {
            return _dlist.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (index == 0)
            {
                _dlist.PushFirst(item);
            }
            else
            {
                _dlist.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            _dlist.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _dlist[index];
            }
            set
            {
                _dlist[index] = value;
            }
        }

        public void Add(T item)
        {
            _dlist.PushLast(item);
        }

        public void Clear()
        {
            _dlist = new InternalDList<T>(5);
        }

        public bool Contains(T item)
        {
            return _dlist.Contains(item);
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dlist.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dlist.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dlist.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return _dlist.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dlist.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dlist.GetEnumerator();
        }

        #endregion

        #region IDeque<T>

        public void PushFirst(T item)
        {
            _dlist.PushFirst(item);
        }

        public void PushLast(T item)
        {
            _dlist.PushLast(item);
        }

        public bool TryPopFirst(out T item)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekFirst(out T item)
        {
            throw new NotImplementedException();
        }

        public bool TryPopLast(out T item)
        {
            throw new NotImplementedException();
        }

        public bool TryPeekLast(out T item)
        {
            throw new NotImplementedException();
        }

        public T First
        {
            get
            {
                if (_dlist.Count > 0)
                {
                    return _dlist[0];
                }
                else
                {
                    throw new IndexOutOfRangeException("Collection contains no elements");
                }
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public T Last
        {
            get
            {
                if (_dlist.Count > 0)
                {
                    return _dlist[_dlist.Count-1];
                }
                else
                {
                    throw new IndexOutOfRangeException("Collection contains no elements");
                }
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsEmpty
        {
            get { return _dlist.Count <= 0; }
        }

        #endregion

        public void InsertRange(int index, ICollection<T> collection)
        {
            if (index == 0)
            {
                _dlist.PushFirst(collection);
            }
            else
            {
                _dlist.InsertRange(index, collection);
            }
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            _dlist.InsertRange(index, items);
        }

        public void AddRange(ICollection<T> collection)
        {
            _dlist.AddRange(collection);
        }

        public void AddRange(IEnumerable<T> items)
        {
            _dlist.AddRange(items);
        }

        public void RemoveRange(ICollection<T> collection)
        {
            _dlist.RemoveRange(collection);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            _dlist.RemoveRange(items);
        }

        /// <summary>
        /// Removes all items starting from the selected index
        /// </summary>
        /// <param name="startIndex"></param>
        public void RemoveRange(int startIndex)
        {
            _dlist.RemoveRange(startIndex, -1);
        }

        public void RemoveRange(int startIndex, int count)
        {
            _dlist.RemoveRange(startIndex, count);
        }

        public T this[int index, T defaultValue]
        {
            get
            {
                if (index < _dlist.Count)
                {
                    return _dlist[index];
                }
                return defaultValue;
            }
        }

        public int Capacity
        {
            get
            {
                return _dlist.Capacity;
            }
            set
            {
                _dlist.Capacity = value;
            }
        }



        int IList.Add(object value)
        {
            _dlist.Add((T)value);
            return _dlist.Count - 1;
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return _dlist.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return _dlist.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            _dlist.Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            _dlist.Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            _dlist.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return _dlist[index];
            }
            set
            {
                _dlist[index] = (T)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null || array.Length < Count)
                throw new ArgumentOutOfRangeException(nameof(array));
            if (index < 0 || array.Length - index < Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            foreach (object obj in this)
                array.SetValue(obj, index++);
        }

        int ICollection.Count
        {
            get { return _dlist.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }
    }

    internal class DList : DList<object>, IList
    {

        public new int Add(object obj)
        {
            base.Add(obj);
            return Count - 1;
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public new void Remove(object obj)
        {
            base.Remove(obj);
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null || array.Length < Count)
                throw new ArgumentOutOfRangeException(nameof(array));
            if (index < 0 || array.Length - index < Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            foreach (object obj in this)
                array.SetValue(obj, index++);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }
    }
}
