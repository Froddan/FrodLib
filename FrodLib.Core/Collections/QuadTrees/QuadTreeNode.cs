using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FrodLib.Collections.QuadTrees
{
    [DebuggerDisplay("Boundary = {ItemBoundary}, Item = {Item}")]
    public struct QuadTreeNode<T>
    {
        public T Item { get; private set; }
        public Rect ItemBoundary { get; private set; }
        public Point ItemCenterPoint { get; private set; }

        public QuadTreeNode(T item, Rect itemBoundary)
            : this()
        {
            Item = item;
            ItemBoundary = itemBoundary;
            ItemCenterPoint = new Point(itemBoundary.X + itemBoundary.Width / 2, itemBoundary.Y + itemBoundary.Height / 2);
        }
    }
}
