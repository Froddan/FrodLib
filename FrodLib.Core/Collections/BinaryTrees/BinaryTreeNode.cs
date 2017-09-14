using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrodLib.Collections;

namespace FrodLib.Collections.BinaryTrees
{
    public class BinaryTreeNode<T> : Node<T>
    {
        public BinaryTreeNode<T> Parent { get; internal set; }

        private const int LeftChildIndex = 0;
        private const int RightChildIndex = 1;

        public BinaryTreeNode<T> LeftChild
        {
            get
            {
                return (BinaryTreeNode<T>)Neighbors[LeftChildIndex];
            }
            internal set
            {
                Neighbors[LeftChildIndex] = value;
                if (Neighbors[LeftChildIndex] != null)
                {
                    ((BinaryTreeNode<T>)Neighbors[LeftChildIndex]).Parent = this;
                }
            }
        }


        public BinaryTreeNode<T> RightChild
        {
            get
            {
                return (BinaryTreeNode<T>)Neighbors[RightChildIndex];
            }
            internal set
            {
                Neighbors[RightChildIndex] = value;
                if (Neighbors[RightChildIndex] != null)
                {
                    ((BinaryTreeNode<T>)Neighbors[RightChildIndex]).Parent = this;
                }
            }
        }

        public bool HasChildren
        {
            get
            {
                return LeftChild != null || RightChild != null;
            }
        }

        public BinaryTreeNode(T item) : base(item, new NodeList<T>(2))
        {
            Value = item;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override void Clear()
        {
            Value = default(T);

            if (LeftChild != null)
            {
                LeftChild.Clear();
                LeftChild = null;
            }
            if (RightChild != null)
            {
                RightChild.Clear();
                RightChild = null;
            }
        }
    }
}
