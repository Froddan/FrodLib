using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using FrodLib.Collections.BinaryTrees.Enumerators;

namespace FrodLib.Collections.BinaryTrees
{
    public abstract class AbstractBinaryTree<T> : IBinaryTree<T>, IDisposable
    {
        protected IComparer<T> comparer;

        protected AbstractBinaryTree(IComparer<T> comparer)
        {
            if (comparer != null)
            {
                this.comparer = comparer;
            }
            else
            {
                this.comparer = Comparer<T>.Default;
            }
        }

        /// <summary>
        /// Specifies the mode of scanning through the tree
        /// </summary>
        public enum TraversalMode
        {
            InOrder = 0,
            PostOrder,
            PreOrder
        }

        private TraversalMode traversalMode = TraversalMode.InOrder;

        /// <summary>
        /// Gets or sets the traversal mode of the tree
        /// </summary>
        public virtual TraversalMode TraversalOrder
        {
            get { return traversalMode; }
            set { traversalMode = value; }
        }

        protected BinaryTreeNode<T> root;
        internal BinaryTreeNode<T> Root
        {
            get
            {
                return root;
            }
        }

        /// <summary>
        /// Returns the height of the subtree rooted at the parameter value
        /// </summary>
        public virtual int GetHeight(T value)
        {
            //Find the value's node in tree
            BinaryTreeNode<T> valueNode = this.FindNode(value);
            if (value != null)
                return this.GetHeight(valueNode);
            else
                return 0;
        }

        /// <summary>
        /// Returns the height of the subtree rooted at the parameter node
        /// </summary>
        public virtual int GetHeight(BinaryTreeNode<T> startNode)
        {
            if (startNode == null)
                return 0;
            else
                return 1 + Math.Max(GetHeight(startNode.LeftChild), GetHeight(startNode.RightChild));
        }

        public virtual T Find(T item)
        {
            BinaryTreeNode<T> currentNode = this.root; //start at head
            while (currentNode != null)
            {
                int comparedValue = comparer.Compare(currentNode.Value, item);
                if (comparedValue == 0) //parameter value found
                {
                    return currentNode.Value;
                }
                else
                {
                    //Search left if the value is smaller than the current node
                    bool searchLeft = comparedValue >= 0;

                    if (searchLeft)
                    {
                        currentNode = currentNode.LeftChild; //search left
                    }
                    else
                    {
                        currentNode = currentNode.RightChild; //search right
                    }
                }
            }

            return default(T); //not found
        }

        protected BinaryTreeNode<T> FindNode(T item)
        {
            BinaryTreeNode<T> currentNode = this.root; //start at head
            while (currentNode != null)
            {
                if (comparer.Compare(currentNode.Value, item) == 0) //parameter value found
                    return currentNode;
                else
                {
                    int comparedValue = comparer.Compare(currentNode.Value, item);
                    //Search left if the value is smaller than the current node
                    bool searchLeft = comparedValue >= 0;

                    if (searchLeft)
                        currentNode = currentNode.LeftChild; //search left
                    else
                        currentNode = currentNode.RightChild; //search right
                }
            }

            return null; //not found
        }


        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// Returns an enumerator to scan through the elements stored in tree.
        /// The enumerator uses the traversal set in the TraversalMode property.
        /// </summary>
        public virtual IEnumerator<T> GetEnumerator()
        {
            switch (this.TraversalOrder)
            {
                case TraversalMode.InOrder:
                    return GetInOrderEnumerator();
                case TraversalMode.PostOrder:
                    return GetPostOrderEnumerator();
                case TraversalMode.PreOrder:
                    return GetPreOrderEnumerator();
                default:
                    return GetInOrderEnumerator();
            }
        }



        /// <summary>
        /// Returns an enumerator that visits node in the order: left child, parent, right child
        /// </summary>
        public virtual IEnumerator<T> GetInOrderEnumerator()
        {
            return new BinaryTreeInOrderEnumerator<T>(this);
        }

        /// <summary>
        /// Returns an enumerator that visits node in the order: left child, right child, parent
        /// </summary>
        public virtual IEnumerator<T> GetPostOrderEnumerator()
        {
            return new BinaryTreePostOrderEnumerator<T>(this);
        }

        /// <summary>
        /// Returns an enumerator that visits node in the order: parent, left child, right child
        /// </summary>
        public virtual IEnumerator<T> GetPreOrderEnumerator()
        {
            return new BinaryTreePreOrderEnumerator<T>(this);
        }

        public abstract bool Insert(T item);
        public abstract bool Remove(T item);
        public abstract void Clear();

        void IDisposable.Dispose()
        {
            Clear();
        }

        void ICollection<T>.Add(T item)
        {
            Insert(item);
        }

        public bool Contains(T item)
        {
            return FindNode(item) != null;
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

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
