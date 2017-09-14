using FrodLib.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Collections
{

    public class PriorityQueue<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        internal List<KeyValuePair<int, LinkedList<T>>> m_queues = new List<KeyValuePair<int, LinkedList<T>>>();
        private int m_count;

        /// <summary>
        /// Enqueues an item with a priority.
        /// <para>The lower number the higher priority</para>
        /// <para>When the priority is lesser then 1 it will be set to 1 as it is the highest</para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public virtual void Enqueue(T item, int priority)
        {
            if(priority < 1)
            {
                priority = 1;
            }

            bool itemAdded = false;
            for (int i = 0; i < m_queues.Count; i++)
            {
                if (m_queues[i].Key == priority)
                {
                    m_queues[i].Value.AddLast(item);
                    itemAdded = true;
                    m_count++;
                    break;
                }
                else if (m_queues[i].Key > priority)
                {
                    var keyPair = new KeyValuePair<int, LinkedList<T>>(priority, new LinkedList<T>());
                    keyPair.Value.AddLast(item);
                    m_queues.Insert(i, keyPair);
                    itemAdded = true;
                    m_count++;
                    break;
                }
            }

            if (!itemAdded)
            {
                var keyPair = new KeyValuePair<int, LinkedList<T>>(priority, new LinkedList<T>());
                keyPair.Value.AddLast(item);
                m_queues.Add(keyPair);
                m_count++;
            }
        }

        public virtual T Dequeue()
        {
            if(m_queues.Any())
            {
                var keyPair = m_queues[0];
                T itemToBeReturned = keyPair.Value.First.Value;
                keyPair.Value.RemoveFirst();
                if(!keyPair.Value.Any())
                {
                    m_queues.Remove(keyPair);
                }
                m_count--;
                return itemToBeReturned;
            }
            else
            {
                throw new InvalidOperationException(StringResources.QueueIsEmpty);
            }
        }

        public virtual T Peek()
        {
            if (m_queues.Any())
            {
                return m_queues[0].Value.First.Value;
            }
            else
            {
                throw new InvalidOperationException(StringResources.QueueIsEmpty);
            }
        }

       
        /// <summary>
        /// Adds an item with the lowest priority
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            Enqueue(item, int.MaxValue);
        }

        public void Clear()
        {
            m_queues.Clear();
            m_count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < m_queues.Count; i++)
            {
                var innerQueue = m_queues[i].Value;
                if(innerQueue.Contains(item))
                {
                    return true;
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

        public int Count
        {
            get { return m_count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            for(int i = 0; i<m_queues.Count; i++)
            {
                var queue = m_queues[i];
                if (queue.Value.Contains(item))
                {
                    bool succes = queue.Value.Remove(item);
                    if (!queue.Value.Any())
                    {
                        m_queues.RemoveAt(i);
                    }
                    if(succes)
                    {
                        m_count--;
                    }
                    return succes;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new PriorityQueueEnumerator<T>(m_queues);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Priority Queue: [");
            for (int i = 0; i < m_queues.Count; i++)
            {
                var innerQueue = m_queues[i].Value;
                for (int j = 0; j < innerQueue.Count; j++)
                {
                    sb.Append(innerQueue.ElementAt(j));

                    if (i != m_queues.Count - 1 && j != innerQueue.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append("]");
            return base.ToString();
        }
    }

    internal class PriorityQueueEnumerator<T> : IEnumerator<T>
    {
        private List<KeyValuePair<int, LinkedList<T>>> m_innerQueue;
        private T m_current;
        private int i, j;

        internal PriorityQueueEnumerator(List<KeyValuePair<int, LinkedList<T>>> innerQueue)
        {
            this.m_innerQueue = innerQueue;
        }

        public T Current
        {
            get { return m_current; }
        }

        object IEnumerator.Current
        {
            get { return m_current; }
        }

        public bool MoveNext()
        {
            if (m_innerQueue == null) return false;
            for(; i< m_innerQueue.Count; i++)
            {
                var innerInnerQueue = m_innerQueue[i].Value;
                if (j < innerInnerQueue.Count)
                {
                    m_current = innerInnerQueue.ElementAt(j);
                    j++;
                    return true;
                }
                j = 0;
            }
            return false;
        }

        public void Reset()
        {
            m_current = default(T);
            i = 0;
            j = 0;
        }

        public void Dispose()
        {
            m_innerQueue = null;
            Reset();
        }
    }
}
