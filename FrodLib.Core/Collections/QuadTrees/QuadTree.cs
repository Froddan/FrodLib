using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FrodLib.Extensions;

namespace FrodLib.Collections.QuadTrees
{
    [DebuggerDisplay("Boundary = {Boundary}, Count = {Count}")]
    public class QuadTree<T> : IQuadTree<T>, IDisposable
    {
        protected internal List<QuadTreeNode<T>> leafNodes;
        protected internal QuadTree<T> northEast;
        protected internal QuadTree<T> northWest;
        protected internal QuadTree<T> southEast;
        protected internal QuadTree<T> southWest;
        protected readonly int QT_NODE_CAPACITY;
        protected List<QuadTreeNode<T>> nodesOutOfBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{T}" /> class.
        /// </summary>
        public QuadTree(Rect boundary)
            : this(1, boundary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{T}" /> class.
        /// </summary>
        public QuadTree(double x, double y, double width, double height)
            : this(1, new Rect(x, y, width, height))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{T}" /> class.
        /// </summary>
        /// <param name="maxNumberOfLeafNodes">The max number of leaf nodes.</param>
        /// <param name="boundary">The boundary.</param>
        public QuadTree(int maxNumberOfLeafNodes, Rect boundary)
        {
            this.Boundary = boundary;
            QT_NODE_CAPACITY = maxNumberOfLeafNodes;
            leafNodes = new List<QuadTreeNode<T>>(QT_NODE_CAPACITY);
            nodesOutOfBounds = new List<QuadTreeNode<T>>(QT_NODE_CAPACITY);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{T}" /> class.
        /// </summary>
        /// <param name="maxNumberOfLeafNodes">The max number of leaf nodes.</param>
        /// <param name="x">The x position of the boundary</param>
        /// <param name="y">The y position of the boundary</param>
        /// <param name="width">The width of the boundary</param>
        /// <param name="height">The height of the boundary</param>
        public QuadTree(int maxNumberOfLeafNodes, double x, double y, double width, double height)
            : this(maxNumberOfLeafNodes, new Rect(x, y, width, height))
        {
        }

        /// <summary>
        /// The boundary for this quad tree
        /// </summary>
        public Rect Boundary { get; private set; }

        /// <summary>
        /// Returns the number of items that has been added to the quad tree, including items that have fallen out of bounds
        /// </summary>
        public virtual int Count
        {
            get
            {
                int count = leafNodes.Count;
                count += nodesOutOfBounds.Count;
                if (northWest != null)
                {
                    count += northWest.Count;
                    count += northEast.Count;
                    count += southWest.Count;
                    count += southEast.Count;
                }
                return count;
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
        /// Returns the number of nodes that has been moved out of bounds by resizeing
        /// <para>To add the nodes back into the boundary, resize the quad tree so the nodes fits again</para>
        /// </summary>
        public virtual int NodesOutOfBounds
        {
            get
            {
                int count = nodesOutOfBounds.Count;
                if (northWest != null)
                {
                    count += northWest.NodesOutOfBounds;
                    count += northEast.NodesOutOfBounds;
                    count += southWest.NodesOutOfBounds;
                    count += southEast.NodesOutOfBounds;
                }
                return count;
            }
        }

        
        /// <summary>
        /// Adds the item to the quad tree with it's boundary as position and size
        /// </summary>
        /// <param name="item"></param>
        public bool Insert(IBoundary<T> item)
        {
            return Insert(item.Implementation, item.Boundary);
        }

        /// <summary>
        /// Clears the entire quad tree
        /// </summary>
        public virtual void Clear()
        {
            ClearChildNode(ref northWest);
            ClearChildNode(ref northEast);
            ClearChildNode(ref southWest);
            ClearChildNode(ref southEast);

            leafNodes.Clear();
            nodesOutOfBounds.Clear();
        }

        /// <summary>
        /// Checks if the quad tree contains an item
        /// <para>check both items in bounds and out of bounds</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the tree contains the item</returns>
        public virtual bool Contains(T item)
        {
            if (this.leafNodes.Any(n => n.Item.Equals(item)))
            {
                return true;
            }
            else if (this.nodesOutOfBounds.Any(n => n.Item.Equals(item)))
            {
                return true;
            }
            else if (northWest != null)
            {
                if (northWest.Contains(item)) return true;
                else if (northEast.Contains(item)) return true;
                else if (southWest.Contains(item)) return true;
                else if (southEast.Contains(item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the quad tree to an array.
        /// </summary>
        /// <param name="array">The array.</param>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the quad tree to an array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        public virtual void CopyTo(T[] array, int startIndex)
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
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            foreach (var item in QueryAllInBoundsItems())
            {
                yield return item;
            }
            foreach (var item in QueryItemsThatAreOutOfBounds())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Adds a item in the quad tree at pos (1,1) with width and height = 0
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            Insert(item, new Rect(1, 1, 0, 0));
        }

        void IDisposable.Dispose()
        {
            Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Inserts an item into the quad tree
        /// </summary>
        /// <param name="itemBoundary">The position and size of the item</param>
        /// <param name="item">The item to be inserted</param>
        /// <returns>True if succesfully inserted</returns>
        public bool Insert(T item, Rect itemBoundary)
        {
            return Insert(item, ref itemBoundary);
        }

        /// <summary>
        /// Inserts an item into the quad tree
        /// </summary>
        /// <param name="itemBoundary">The position and size of the item</param>
        /// <param name="item">The item to be inserted</param>
        /// <returns>True if succesfully inserted</returns>
        public virtual bool Insert(T item, ref Rect itemBoundary)
        {
            QuadTreeNode<T> node = new QuadTreeNode<T>(item, itemBoundary);
            return InsertNode(ref node);
        }

        /// <summary>
        /// Moves the item to its new position.
        /// </summary>
        /// <param name="item">The item to be moved.</param>
        /// <param name="newItemBoundary">The new item boundary.</param>
        /// <returns>True if the item was succesfully moved to it's new location. false if the item doesn't exist or it tries to move the item out of bounds</returns>
        public virtual bool MoveItem(T item, Rect newItemBoundary)
        {
            QuadTreeNode<T> removedNode;
            if (Remove(item, out removedNode))
            {
                QuadTreeNode<T> movedNode = new QuadTreeNode<T>(item, newItemBoundary);
                if (InsertNode(ref movedNode))
                {
                    return true;
                }
                else
                {
                    InsertNode(ref removedNode);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of objects at that specific point
        /// </summary>
        /// <param name="point">The point on where to select items</param>
        /// <returns></returns>
        public List<T> Query(Point point)
        {
            List<T> results = new List<T>();
            Query(new Rect(point, point), results);
            return results;
        }

        /// <summary>
        /// Returns a collection that is inside the selected range
        /// </summary>
        /// <param name="range">The range in where items will be selected</param>
        /// <returns>A collection of items</returns>
        public List<T> Query(Rect range)
        {
            List<T> results = new List<T>();
            Query(ref range, results);
            return results;
        }

        /// <summary>
        /// Returns a collection that is inside the selected range
        /// </summary>
        /// <param name="range">The range in where items will be selected</param>
        /// <returns>A collection of items</returns>
        ///
        public void Query(Rect range, List<T> results)
        {
            if (results != null)
            {
                Query(ref range, results);
            }
        }

        /// <summary>
        /// Returns a collection that is inside the selected range
        /// </summary>
        /// <param name="range">The range in where items will be selected</param>
        /// <returns>A collection of items</returns>
        public virtual void Query(ref Rect range, List<T> results)
        {
            if (results != null)
            {
                if (range.Contains(Boundary))
                {
                    QueryAllInBoundsItems(results);
                }
                else if (!Boundary.IntersectsWith(range))
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < leafNodes.Count; i++)
                    {
                        if (range.IntersectsWith(leafNodes[i].ItemBoundary))
                        {
                            results.Add(leafNodes[i].Item);
                        }
                    }

                    if (northWest == null)
                    {
                        return;
                    }

                    northWest.Query(ref range, results);
                    northEast.Query(ref range, results);
                    southWest.Query(ref range, results);
                    southEast.Query(ref range, results);
                }
            }
        }

        /// <summary>
        /// Gets all the items that are in bounds.
        /// </summary>
        /// <returns></returns>
        public List<T> QueryAllInBoundsItems()
        {
            List<T> results = new List<T>();
            QueryAllInBoundsItems(results);
            return results;
        }

        /// <summary>
        /// Gets all the items that are in bounds.
        /// </summary>
        /// <param name="results">The results.</param>
        public virtual void QueryAllInBoundsItems(List<T> results)
        {
            if (results != null)
            {
                for (int i = 0; i < leafNodes.Count; i++)
                {
                    results.Add(leafNodes[i].Item);
                }

                if (northWest != null)
                {
                    northWest.QueryAllInBoundsItems(results);
                    northEast.QueryAllInBoundsItems(results);
                    southWest.QueryAllInBoundsItems(results);
                    southEast.QueryAllInBoundsItems(results);
                }
            }
        }

        /// <summary>
        /// Queries the tree for all items that has been moved out of bounds
        /// </summary>
        /// <returns></returns>
        public virtual List<T> QueryItemsThatAreOutOfBounds()
        {
            List<T> itemsOutOfBounds = new List<T>();
            itemsOutOfBounds.AddRange(nodesOutOfBounds.Select(n => n.Item));

            if (northWest != null)
            {
                itemsOutOfBounds.AddRange(northWest.QueryItemsThatAreOutOfBounds());
                itemsOutOfBounds.AddRange(northEast.QueryItemsThatAreOutOfBounds());
                itemsOutOfBounds.AddRange(southWest.QueryItemsThatAreOutOfBounds());
                itemsOutOfBounds.AddRange(southEast.QueryItemsThatAreOutOfBounds());
            }

            return itemsOutOfBounds;
        }

        /// <summary>
        /// Deletes an item from the quad tree
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>True if successfully removed</returns>
        public virtual bool Remove(T item)
        {
            QuadTreeNode<T> removedNode;
            return Remove(item, out removedNode);
        }

        /// <summary>
        /// Resizes the boundary of the quad tree and moves the nodes to its new sub quad (if there is any)
        /// <para>Note that some items can end up out of bounds. These items can be queried by calling QueryItemsThatAreOutOfBounds</para>
        /// <para>and can then be moved inside bounds again by calling MoveItem with a boundary inside the QuadTree boundary</para>
        /// </summary>
        /// <param name="newBoundary"></param>
        public virtual void ResizeBoundary(Rect newBoundary)
        {
            if (newBoundary == Boundary) return;

            List<QuadTreeNode<T>> nodes = new List<QuadTreeNode<T>>();
            GetAllNodes(nodes);
            nodes.AddRange(nodesOutOfBounds);
            nodesOutOfBounds.Clear();
            Clear();

            Boundary = newBoundary;

            foreach (var node in nodes)
            {
                var refNode = node;
                if (!InsertNode(ref refNode))
                {
                    nodesOutOfBounds.Add(node);
                }
            }
        }

        protected virtual void GetAllNodes(List<QuadTreeNode<T>> results)
        {
            if (results != null)
            {
                for (int i = 0; i < leafNodes.Count; i++)
                {
                    results.Add(leafNodes[i]);
                }

                if (northWest != null)
                {
                    northWest.GetAllNodes(results);
                    northEast.GetAllNodes(results);
                    southWest.GetAllNodes(results);
                    southEast.GetAllNodes(results);
                }
            }
        }

        protected virtual QuadTree<T> GetDestinationTree(Rect boundary)
        {
            QuadTree<T> destination = this;

            if (northWest.Boundary.Contains(boundary))
            {
                destination = northWest;
            }

            if (northEast.Boundary.Contains(boundary))
            {
                destination = northEast;
            }

            if (southWest.Boundary.Contains(boundary))
            {
                destination = southWest;
            }

            if (southEast.Boundary.Contains(boundary))
            {
                destination = southEast;
            }

            return destination;
        }

        protected virtual void Subdivide()
        {
            double centerX = Boundary.X + Boundary.Width / 2;
            double centerY = Boundary.Y + Boundary.Height / 2;
            Point center = new Point(centerX, centerY);
            northWest = new QuadTree<T>(new Rect(Boundary.TopLeft, center));
            northEast = new QuadTree<T>(new Rect(Boundary.TopRight, center));
            southWest = new QuadTree<T>(new Rect(Boundary.BottomLeft, center));
            southEast = new QuadTree<T>(new Rect(center, Boundary.BottomRight));

            for (int i = 1; i < leafNodes.Count; i++)
            {
                var item = leafNodes[i];
                QuadTree<T> node = GetDestinationTree(item.ItemBoundary);
                if (node != this)
                {
                    node.Insert(item.Item, item.ItemBoundary);
                    leafNodes.Remove(item);
                    i--;
                }
            }
        }

        private void ClearChildNode(ref QuadTree<T> childNode)
        {
            if (childNode != null)
            {
                childNode.Clear();
                childNode = null;
            }
        }

        private bool InsertNode(ref QuadTreeNode<T> node)
        {
            if (!Boundary.Contains(node.ItemCenterPoint))
            {
                return false;
            }

            if (leafNodes.Count < QT_NODE_CAPACITY)
            {
                leafNodes.Add(node);
                return true;
            }

            if (northWest == null)
            {
                Subdivide();
            }

            if (northWest.InsertNode(ref node)) return true;
            else if (northEast.InsertNode(ref node)) return true;
            else if (southWest.InsertNode(ref node)) return true;
            else if (southEast.InsertNode(ref node)) return true;

            // we shouldn't reach this
            return false;
        }

        /// <summary>
        /// Deletes an item from the quad tree
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>True if successfully removed</returns>
        private bool Remove(T item, out QuadTreeNode<T> removedNode)
        {
            //check if the item exist in the out of bounds collection
            if (nodesOutOfBounds.Count > 0)
            {
                int outOfBoundsIndex = nodesOutOfBounds.FindIndex(n => n.Item.Equals(item));
                if (outOfBoundsIndex != -1)
                {
                    removedNode = nodesOutOfBounds[outOfBoundsIndex];
                    nodesOutOfBounds.RemoveAt(outOfBoundsIndex);
                    return true;
                }
            }

            int index = leafNodes.FindIndex(n => n.Item.Equals(item));
            if (index != -1)
            {
                removedNode = leafNodes[index];
                leafNodes.RemoveAt(index);
                return true;
            }
            else
            {
                bool success = false;
                if (northWest != null)
                {
                    if (northWest.Remove(item, out removedNode)) success = true;
                    else if (northEast.Remove(item, out removedNode)) success = true;
                    else if (southWest.Remove(item, out removedNode)) success = true;
                    else if (southEast.Remove(item, out removedNode)) success = true;

                    if (northWest.Count == 0 && northEast.Count == 0 && southWest.Count == 0 && southEast.Count == 0)
                    {
                        northWest = null;
                        northEast = null;
                        southWest = null;
                        southEast = null;
                    }
                }
                else
                {
                    removedNode = default(QuadTreeNode<T>);
                }
                return success;
            }
        }
    }
}