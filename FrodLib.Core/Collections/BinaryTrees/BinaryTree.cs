using System.Collections.Generic;

namespace FrodLib.Collections.BinaryTrees
{
    /// <summary>
    /// Important!!! the tree can be unbalanced
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryTree<T> : AbstractBinaryTree<T>
    {
        public BinaryTree()
            : base(null)
        {
        }

        public BinaryTree(IComparer<T> comparer)
            : base(comparer)
        {
        }

        public BinaryTree(IEnumerable<T> items)
            : base(null)
        {
            InsertRange(items);
        }

        public BinaryTree(IEnumerable<T> items, IComparer<T> comparer)
            : base(comparer)
        {
            InsertRange(items);
        }

        public override int Count
        {
            get
            {
                int count = 0;
                CalculateNumberOfNodes(root, ref count);
                return count;
            }
        }

        public override void Clear()
        {
            root = null;
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Insert(T item)
        {
            if (item == null)
            {
                return false;
            }
            else if (root == null)
            {
                root = CreateNode(item);
                return true;
            }
            else
            {
                var currentNode = root;
                while (currentNode != null)
                {
                    int comparedValue = comparer.Compare(currentNode.Value, item);
                    if (comparedValue >= 0)
                    {
                        var child = currentNode.LeftChild;
                        if (child == null)
                        {
                            var newNode = CreateNode(item);
                            newNode.Parent = currentNode;
                            currentNode.LeftChild = newNode;
                            return true;
                        }
                        else
                        {
                            currentNode = child;
                        }
                    }
                    else if (comparedValue < 0)
                    {
                        var child = currentNode.RightChild;
                        if (child == null)
                        {
                            var newNode = CreateNode(item);
                            newNode.Parent = currentNode;
                            currentNode.RightChild = newNode;
                            return true;
                        }
                        else
                        {
                            currentNode = child;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Inserts the collection into the tree one at the time
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool InsertRange(IEnumerable<T> items)
        {
            bool success = false;
            if (items != null)
            {
                foreach (var item in items)
                {
                    success |= Insert(item);
                }
            }
            return success;
        }

        public override bool Remove(T item)
        {
            if (root == null)
            {
                return false;
            }
            if (root.Value.Equals(item))
            {
                //has no children
                if (!root.HasChildren)
                {
                    root = null;
                }
                else if (root.LeftChild != null && root.RightChild == null) //has only left child
                {
                    root = root.LeftChild;
                }
                else if (root.LeftChild == null && root.RightChild != null) //has only right child
                {
                    root = root.RightChild;
                }
                else if (!root.RightChild.HasChildren) // has no right grand children
                {
                    root.RightChild.LeftChild = root.LeftChild;
                    root = root.RightChild;
                }
                else
                {
                    var predecessor = FindPredecessor(root.LeftChild);
                    RemovePredecessorFromParent(root, predecessor);

                    predecessor.RightChild = root.RightChild;
                    predecessor.LeftChild = root.LeftChild;
                    root = predecessor;
                }
                return true;
            }
            else
            {
                return RemoveNode(root, item);
            }
        }

        protected virtual BinaryTreeNode<T> CreateNode(T item)
        {
            return new BinaryTreeNode<T>(item);
        }

        private void CalculateNumberOfNodes(BinaryTreeNode<T> currentNode, ref int count)
        {
            if (currentNode != null)
            {
                count++;
                CalculateNumberOfNodes(currentNode.LeftChild, ref count);
                CalculateNumberOfNodes(currentNode.RightChild, ref count);
            }
        }

        private BinaryTreeNode<T> FindPredecessor(BinaryTreeNode<T> currentNode)
        {
            while (currentNode != null)
            {
                if (currentNode.RightChild == null)
                {
                    return currentNode;
                }
                else
                {
                    currentNode = currentNode.RightChild;
                }
            }
            return null;
        }

        private bool RemoveNode(BinaryTreeNode<T> currentNode, T item)
        {
            if (currentNode == null)
            {
                return false;
            }
            else
            {
                while (currentNode != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        BinaryTreeNode<T> child = (i == 0) ? currentNode.LeftChild : currentNode.RightChild;
                        if (child != null)
                        {
                            if (comparer.Compare(child.Value, item) == 0)
                            {
                                //has no children
                                if (!child.HasChildren)
                                {
                                    if (i == 0)
                                    {
                                        currentNode.LeftChild = null;
                                    }
                                    else
                                    {
                                        currentNode.RightChild = null;
                                    }
                                }
                                else if (child.LeftChild != null && child.RightChild == null) //has only left child
                                {
                                    if (i == 0)
                                    {
                                        currentNode.LeftChild = child.LeftChild;
                                    }
                                    else
                                    {
                                        currentNode.RightChild = child.LeftChild;
                                    }
                                }
                                else if (child.LeftChild == null && child.RightChild != null) //has only right child
                                {
                                    if (i == 0)
                                    {
                                        currentNode.LeftChild = child.RightChild;
                                    }
                                    else
                                    {
                                        currentNode.RightChild = child.RightChild;
                                    }
                                }
                                else
                                {
                                    var predecessor = FindPredecessor(child.LeftChild);
                                    RemovePredecessorFromParent(root, predecessor);

                                    predecessor.RightChild = child.RightChild;
                                    predecessor.LeftChild = child.LeftChild;

                                    if (i == 0)
                                    {
                                        currentNode.LeftChild = predecessor;
                                    }
                                    else
                                    {
                                        currentNode.RightChild = predecessor;
                                    }
                                }

                                return true;
                            }
                        }
                    }
                    int comparedValue = comparer.Compare(currentNode.Value, item);
                    if (comparedValue >= 0)
                    {
                        var child = currentNode.LeftChild;
                        currentNode = child;
                    }
                    else if (comparedValue < 0)
                    {
                        var child = currentNode.RightChild;
                        currentNode = child;
                    }
                }
            }
            return false;
        }

        private bool RemovePredecessorFromParent(BinaryTreeNode<T> currentNode, BinaryTreeNode<T> predecessor)
        {
            if (currentNode != null)
            {
                if (currentNode.LeftChild != null && currentNode.LeftChild.Equals(predecessor))
                {
                    if (predecessor.RightChild == null && predecessor.LeftChild != null)
                    {
                        currentNode.LeftChild = predecessor.LeftChild;
                    }
                    else
                    {
                        currentNode.LeftChild = null;
                    }
                    return true;
                }
                else if (currentNode.RightChild != null && currentNode.RightChild.Equals(predecessor))
                {
                    currentNode.RightChild = predecessor.LeftChild;
                    return true;
                }
                if (RemovePredecessorFromParent(currentNode.RightChild, predecessor)) return true;
                if (RemovePredecessorFromParent(currentNode.LeftChild, predecessor)) return true;
            }
            return false;
        }
    }
}