using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FrodLib.Collections
{
    public class SkipList<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private SkipListNode<T> _head;
        private Random _random;
        private int _count;
        private bool isAddingCollection;

        protected const double Probability = 0.5d;
        private IComparer<T> comparer = Comparer<T>.Default;

        //public bool SupportInsert { get; set; }

        public SkipList() : this(-1, null, null) { }
        public SkipList(int randomSeed) : this(randomSeed, null, null) { }
        public SkipList(IComparer<T> comparer) : this(-1, null, comparer) { }

        public SkipList(int randomSeed, IComparer<T> comparer)
            : this(randomSeed, null, comparer) { }

        public SkipList(IEnumerable<T> items) : this(-1, items, null) { }

        public SkipList(int randomSeed, IEnumerable<T> items) : this(randomSeed, items, null) { }
        public SkipList(IEnumerable<T> items, IComparer<T> comparer) : this(-1, items, comparer) { }

        public SkipList(int randomSeed, IEnumerable<T> items, IComparer<T> comparer)
        {
            _head = new SkipListNode<T>(1);
            _count = 0;
            if (randomSeed < 0)
            {
                _random = new Random();
            }
            else
            {
                _random = new Random(randomSeed);
            }
            if (comparer != null) this.comparer = comparer;

            AddRange(items);
        }

        /// <summary>
        /// Adds an item to the skiplist
        /// <para>If the item is a duplicate it will NOT be added</para>
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            SkipListNode<T>[] updates = BuildUpdateTable(item);
            SkipListNode<T> current = updates[0];

            if (current[0] != null && comparer.Compare(current[0].Value, item) == 0)
            {
                // duplicate entry
                return;
            }

            SkipListNode<T> node = new SkipListNode<T>(item, ChooseRandomHeight(Height + 1));
            node.IsAddingCollection = isAddingCollection;
            _count++;

            if (node.Height > _head.Height)
            {
                _head.IncrementHeight();
                _head[_head.Height - 1] = node;
            }

            for (int i = 0; i < node.Height; i++)
            {
                if (i < updates.Length)
                {
                    node[i] = updates[i][i];
                    updates[i][i] = node;
                }
            }
        }

        private SkipListNode<T>[] BuildUpdateTable(T item)
        {
            SkipListNode<T>[] updates = new SkipListNode<T>[Height];
            var current = _head;

            for (int i = Height - 1; i >= 0; i--)
            {
                while (current[i] != null && comparer.Compare(current[i].Value, item) < 0)
                {
                    current = current[i];
                }
                updates[i] = current;
            }

            return updates;
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// <para>NOTICE! if a duplicate item already exist in the skiplist it will not be added</para>
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items != null && items.Any())
            {
                BeginAddCollection();
                foreach (var item in items)
                {
                    Add(item);
                }
                FinishAddCollection();
            }
        }

        protected void FinishAddCollection()
        {
            isAddingCollection = false;
            _head.IsAddingCollection = isAddingCollection;
            var current = _head[0];
            while (current != null)
            {
                var prev = _head;
                current.IsAddingCollection = isAddingCollection;
                current.CurrentIndex = prev.CurrentIndex + 1;
                prev = current;
                current = current[0];
            }
        }

        protected void BeginAddCollection()
        {
            isAddingCollection = true;
            _head.IsAddingCollection = isAddingCollection;
            var current = _head[0];
            while (current != null)
            {
                current.IsAddingCollection = isAddingCollection;
                current = current[0];
            }
        }

        /// <summary>
        /// Clears the collection of all its elements
        /// </summary>
        public void Clear()
        {
            var current = _head[0];
            while (current != null)
            {
                var previous = current;
                current = current[0];
                previous.Clear();
            }
            _head[0] = null;
            var height = _head.Height;
            for (int i = 0; i < height; i++)
            {
                _head.DecrementHeight();
            }
            _count = 0;
        }

        /// <summary>
        /// Checks if the collection contains the item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            SkipListNode<T> current = _head;
            for (int i = Height - 1; i >= 0; i--)
            {
                while (current[i] != null)
                {
                    int result = comparer.Compare(current[i].Value, item);
                    if (result == 0)
                    {
                        return true;
                    }
                    else if (result < 0)
                    {
                        current = current[i];
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return false;
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int startIndex)
        {
            IEnumerator<T> enumerator = this.GetEnumerator();

            for (int i = startIndex; i < array.Length; i++)
            {
                if (enumerator.MoveNext())
                    array[i] = enumerator.Current;
                else
                    break;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Returns the current height of the skip list
        /// </summary>
        public int Height
        {
            get
            {
                return _head.Height;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            if (_count == 0)
            {
                return false;
            }

            SkipListNode<T>[] updates = BuildUpdateTable(item);
            SkipListNode<T> current = updates[0][0];

            if (current != null && comparer.Compare(current.Value, item) == 0)
            {
                _count--;

                for (int i = 0; i < _head.Height; i++)
                {
                    if (updates[i][i] != current)
                    {
                        break;
                    }
                    else
                    {
                        updates[i][i] = current[i];
                    }
                }

                if (_head[Height - 1] == null)
                {
                    _head.DecrementHeight();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_head.HasNeighbors)
            {
                SkipListNode<T> current = _head[0];
                while (current != null)
                {
                    yield return current.Value;
                    current = current[0];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected int ChooseRandomHeight(int maxLevel)
        {
            int level = 1;
            while (_random.NextDouble() < Probability && level < maxLevel)
            {
                level++;
            }
            return level;
        }

        /// <summary>
        /// Returns the index of the item
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            SkipListNode<T> current = _head;
            for (int i = Height - 1; i >= 0; i--)
            {
                while (current[i] != null)
                {
                    int result = comparer.Compare(current[i].Value, item);
                    if (result == 0)
                    {
                        return current[i].CurrentIndex;
                    }
                    else if (result < 0)
                    {
                        current = current[i];
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Removes the item at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public void RemoveAt(int index)
        {
            if (index >= _count)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                IComparer<int> indexComparer = Comparer<int>.Default;
                SkipListNode<T> current = _head;
                for (int i = Height - 1; i >= 0; i--)
                {
                    while (current[i] != null)
                    {
                        int result = indexComparer.Compare(current[i].CurrentIndex, index);
                        if (result == 0)
                        {
                            Remove(current[i].Value);
                            return;
                        }
                        else if (result < 0)
                        {
                            current = current[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the value at the index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index >= _count)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    IComparer<int> indexComparer = Comparer<int>.Default;
                    SkipListNode<T> current = _head;
                    for (int i = Height - 1; i >= 0; i--)
                    {
                        while (current[i] != null)
                        {

                            int result = indexComparer.Compare(current[i].CurrentIndex, index);
                            if (result == 0)
                            {
                                return current[i].Value;
                            }
                            else if (result < 0)
                            {
                                current = current[i];
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    return default(T);
                }
            }
        }
    }
}
