using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Collections.BinaryTrees
{
    public class AVLTree<T> : BinaryTree<T>
    {
        public AVLTree()
        {
        }

        public AVLTree(IComparer<T> comparer)
            : base(null, comparer)
        {

        }

        public AVLTree(IEnumerable<T> items)
            : base(items, null)
        {
            InsertRange(items);
        }

        public AVLTree(IEnumerable<T> items, IComparer<T> comparer)
            : base(items, comparer)
        {

        }


        public override bool Insert(T item)
        {
            if (base.Insert(item))
            {
                lastAddedItem = item;
                if (!isAddingCollection)
                {
                    var node = FindNode(item) as AVLTreeNode<T>;
                    //Balance every node going up, starting with the parent
                    AVLTreeNode<T> parentNode = node.Parent;

                    while (parentNode != null)
                    {
                        int balance = this.GetBalance(parentNode);
                        if (Math.Abs(balance) == 2) //-2 or 2 is unbalanced
                        {
                            //Rebalance tree
                            this.BalanceAt(parentNode, balance);
                        }

                        parentNode = parentNode.Parent; //keep going up
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isAddingCollection;

        public override bool InsertRange(IEnumerable<T> items)
        {
            bool success = false;

            if (items != null)
            {
                BeginAddCollection();
                success = base.InsertRange(items);
                FinishedAddCollection();
            }
            return success;
        }

        private T lastAddedItem;

        protected void FinishedAddCollection()
        {
            if (isAddingCollection)
            {
                isAddingCollection = false;
                var node = FindNode(lastAddedItem) as AVLTreeNode<T>;

                if(node == null)
                {
                    return;
                }

                //Balance every node going up, starting with the parent
                AVLTreeNode<T> parentNode = node.Parent;

                while (parentNode != null)
                {
                    int balance = this.GetBalance(parentNode);
                    if (Math.Abs(balance) == 2) //-2 or 2 is unbalanced
                    {
                        //Rebalance tree
                        this.BalanceAt(parentNode, balance);
                    }

                    parentNode = parentNode.Parent; //keep going up
                }
            }
        }

        protected void BeginAddCollection()
        {
            isAddingCollection = true;
        }

        /// <summary>
        /// Removes a given node from the tree and rebalances the tree if necessary.
        /// </summary>
        public override bool Remove(T item)
        {
            var valueNode = FindNode(item) as AVLTreeNode<T>;

            //Save reference to the parent node to be removed
            AVLTreeNode<T> parentNode = valueNode.Parent;

            //Remove the node as usual
            bool removed = base.Remove(item);

            if (!removed)
                return false; //removing failed, no need to rebalance
            else
            {
                //Balance going up the tree
                while (parentNode != null)
                {
                    int balance = this.GetBalance(parentNode);

                    if (Math.Abs(balance) == 1) //1, -1
                        break; //height hasn't changed, can stop
                    else if (Math.Abs(balance) == 2) //2, -2
                    {
                        //Rebalance tree
                        this.BalanceAt(parentNode, balance);
                    }

                    parentNode = parentNode.Parent;
                }

                return true;
            }
        }


        /// <summary>
        /// Balances an AVL Tree node
        /// </summary>
        protected virtual void BalanceAt(AVLTreeNode<T> node, int balance)
        {
            if (balance == 2) //right outweighs
            {
                int rightBalance = GetBalance(node.RightChild);

                if (rightBalance == 1 || rightBalance == 0)
                {
                    //Left rotation needed
                    RotateLeft(node);
                }
                else if (rightBalance == -1)
                {
                    //Right rotation needed
                    RotateRight(node.RightChild);

                    //Left rotation needed
                    RotateLeft(node);
                }
            }
            else if (balance == -2) //left outweighs
            {
                int leftBalance = GetBalance(node.LeftChild);
                if (leftBalance == 1)
                {
                    //Left rotation needed
                    RotateLeft(node.LeftChild);

                    //Right rotation needed
                    RotateRight(node);
                }
                else if (leftBalance == -1 || leftBalance == 0)
                {
                    //Right rotation needed
                    RotateRight(node);
                }
            }
        }

        /// <summary>
        /// Determines the balance of a given node
        /// </summary>
        protected virtual int GetBalance(AVLTreeNode<T> node)
        {
            //Balance = right child's height - left child's height
            return this.GetHeight(node.RightChild) - this.GetHeight(node.LeftChild);
        }

        /// <summary>
        /// Rotates a node to the left within an AVL Tree
        /// </summary>
        protected virtual void RotateLeft(AVLTreeNode<T> node)
        {
            if (node == null)
                return;

            AVLTreeNode<T> pivot = node.RightChild;

            if (pivot == null)
                return;
            else
            {
                AVLTreeNode<T> nodeParent = node.Parent; //original parent of root node
                bool isLeftChild = (nodeParent != null) && nodeParent.LeftChild == node; //whether the root was the parent's left node
                bool makeTreeRoot = root == node; //whether the root was the root of the entire tree

                //Rotate
                node.RightChild = pivot.LeftChild;
                pivot.LeftChild = node;

                //Update parents
                node.Parent = pivot;
                pivot.Parent = nodeParent;

                if (node.RightChild != null)
                    node.RightChild.Parent = node;

                //Update the entire tree's Root if necessary
                if (makeTreeRoot)
                    root = pivot;

                //Update the original parent's child node
                if (isLeftChild)
                    nodeParent.LeftChild = pivot;
                else
                    if (nodeParent != null)
                        nodeParent.RightChild = pivot;
            }
        }

        /// <summary>
        /// Rotates a node to the right within an AVL Tree
        /// </summary>
        protected virtual void RotateRight(AVLTreeNode<T> node)
        {
            if (node == null)
                return;

            AVLTreeNode<T> pivot = node.LeftChild;

            if (pivot == null)
                return;
            else
            {
                AVLTreeNode<T> nodeParent = node.Parent; //original parent of root node
                bool isLeftChild = (nodeParent != null) && nodeParent.LeftChild == node; //whether the root was the parent's left node
                bool makeTreeRoot = root == node; //whether the root was the root of the entire tree

                //Rotate
                node.LeftChild = pivot.RightChild;
                pivot.RightChild = node;

                //Update parents
                node.Parent = pivot;
                pivot.Parent = nodeParent;

                if (node.LeftChild != null)
                    node.LeftChild.Parent = node;

                //Update the entire tree's Root if necessary
                if (makeTreeRoot)
                    root = pivot;

                //Update the original parent's child node
                if (isLeftChild)
                    nodeParent.LeftChild = pivot;
                else
                    if (nodeParent != null)
                        nodeParent.RightChild = pivot;
            }
        }

        protected override BinaryTreeNode<T> CreateNode(T item)
        {
            return new AVLTreeNode<T>(item);
        }

        public override void Clear()
        {
            base.Clear();
            lastAddedItem = default(T);
        }
    }
}
