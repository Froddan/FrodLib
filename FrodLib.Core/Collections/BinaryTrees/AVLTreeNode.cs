using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Collections.BinaryTrees
{
    public class AVLTreeNode<T> : BinaryTreeNode<T>
    {

        public AVLTreeNode(T item) : base(item)
        {

        }

        public new AVLTreeNode<T> Parent
        {
            get
            {
                return (AVLTreeNode<T>)base.Parent;
            }
            internal set
            {
                base.Parent = value;
            }
        }

        public new AVLTreeNode<T> LeftChild
        {
            get
            {
                return (AVLTreeNode<T>)base.LeftChild;
            }
            internal set
            {
                base.LeftChild = value;
            }
        }

        public new AVLTreeNode<T> RightChild
        {
            get
            {
                return (AVLTreeNode<T>)base.RightChild;
            }
            internal set
            {
                base.RightChild = value;
            }
        }
    }
}
