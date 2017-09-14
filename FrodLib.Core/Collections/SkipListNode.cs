using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FrodLib.Collections
{

    internal class SkipListNode<T> : Node<T>
    {

        internal int CurrentIndex { get; set; }

        public int Height
        {
            get
            {
                return base.Neighbors.Count;
            }
        }

        public SkipListNode(int height)
        {
            base.Neighbors = new SkipListNodeList<T>(height);
            CurrentIndex = -1;
        }

        public SkipListNode(T value, int height) : base(value)
        {
            base.Neighbors = new SkipListNodeList<T>(height);
            CurrentIndex = -1;
        }

        internal SkipListNode<T> this[int index]
        {
            get
            {
                return (SkipListNode<T>)base.Neighbors[index];
            }
            set
            {
                base.Neighbors[index] = value;
                if (value != null && index == 0 && !IsAddingCollection)
                {
                    var newIndex = CurrentIndex + 1;
                    var current = value;
                    while (current != null)
                    {
                        current.CurrentIndex = newIndex++;
                        current = current[0];
                    }
                }
            }
        }


        internal void IncrementHeight()
        {
            ((SkipListNodeList<T>)Neighbors).IncrementHeight();
        }

        internal void DecrementHeight()
        {
            ((SkipListNodeList<T>)Neighbors).DecrementHeight();
        }

        public bool IsAddingCollection { get; set; }
    }

}
