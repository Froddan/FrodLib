using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FrodLib.Collections.BinaryTrees.Enumerators
{
    /// <summary>
    /// Returns an preorder-traversal enumerator for the tree values
    /// </summary>
    internal class BinaryTreePreOrderEnumerator<T> : IEnumerator<T>
    {
        private BinaryTreeNode<T> current;
        private AbstractBinaryTree<T> tree;
        internal Queue<BinaryTreeNode<T>> traverseQueue;

        public BinaryTreePreOrderEnumerator(AbstractBinaryTree<T> tree)
        {
            this.tree = tree;

            //Build queue
            traverseQueue = new Queue<BinaryTreeNode<T>>();
            VisitNode(this.tree.Root);
        }

        private void VisitNode(BinaryTreeNode<T> node)
        {
            if (node == null)
                return;
            else
            {
                traverseQueue.Enqueue(node);
                VisitNode(node.LeftChild);
                VisitNode(node.RightChild);
            }
        }

        public T Current
        {
            get { return current.Value; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            current = null;
            tree = null;
        }

        public void Reset()
        {
            current = null;
        }

        public bool MoveNext()
        {
            if (traverseQueue.Count > 0)
                current = traverseQueue.Dequeue();
            else
                current = null;

            return (current != null);
        }
    }
}
