using System.Collections.Generic;
using System.Threading;
using FrodLib.Utils;

#if !USE_FROD_READ_WRITE_LOCK_IMPLEMENTATION
using FrodReaderWriterLock = System.Threading.ReaderWriterLockSlim;
using LockRecursionPolicy = System.Threading.LockRecursionPolicy;
#endif

namespace FrodLib.Collections.QuadTrees
{

    /// <summary>
    /// A more thread safe version of the quad tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrantQuadTree<T> : QuadTree<T>
    {
        private FrodReaderWriterLock readWriteLock = new FrodReaderWriterLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrantQuadTree{T}" /> class.
        /// </summary>
        public ConcurrantQuadTree(Rect boundary)
            : base(1, boundary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrantQuadTree{T}" /> class.
        /// </summary>
        public ConcurrantQuadTree(double x, double y, double width, double height)
            : base(1, new Rect(x, y, width, height))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrantQuadTree{T}" /> class.
        /// </summary>
        /// <param name="maxNumberOfLeafNodes">The max number of leaf nodes.</param>
        /// <param name="boundary">The boundary.</param>
        public ConcurrantQuadTree(int maxNumberOfLeafNodes, Rect boundary)
            : base(maxNumberOfLeafNodes, boundary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrantQuadTree{T}" /> class.
        /// </summary>
        /// <param name="maxNumberOfLeafNodes">The max number of leaf nodes.</param>
        /// <param name="x">The x position of the boundary</param>
        /// <param name="y">The y position of the boundary</param>
        /// <param name="width">The width of the boundary</param>
        /// <param name="height">The height of the boundary</param>
        public ConcurrantQuadTree(int maxNumberOfLeafNodes, double x, double y, double width, double height)
            : base(maxNumberOfLeafNodes, new Rect(x, y, width, height))
        {
            
        }

        /// <inheritdoc />
        public override int Count
        {
            get
            {

                try
                {
                    readWriteLock.EnterReadLock();
                    return base.Count;
                }
                finally
                {
                    readWriteLock.ExitReadLock();
                }
            }
        }

        /// <inheritdoc />
        public override int NodesOutOfBounds
        {
            get
            {
                try
                {
                    readWriteLock.EnterReadLock();
                    return base.NodesOutOfBounds;
                }
                finally
                {
                    readWriteLock.ExitReadLock();
                }
            }
        }

        /// <inheritdoc />
        public override void Clear()
        {
            try
            {
                readWriteLock.EnterWriteLock();
                base.Clear();
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public override bool Contains(T item)
        {
            try
            {
                readWriteLock.EnterReadLock();
                return base.Contains(item);
            }
            finally
            {
                readWriteLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public override void CopyTo(T[] array, int startIndex)
        {
            try
            {
                readWriteLock.EnterReadLock();
                base.CopyTo(array, startIndex);
            }
            finally
            {
                readWriteLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public override bool Insert(T item, ref Rect itemBoundary)
        {
            try
            {
                readWriteLock.EnterWriteLock();
                return base.Insert(item, ref itemBoundary);
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public override bool MoveItem(T item, Rect newItemBoundary)
        {
            try
            {
                readWriteLock.EnterWriteLock();
                return base.MoveItem(item, newItemBoundary);
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public override void Query(ref Rect range, List<T> results)
        {
            try
            {
                readWriteLock.EnterReadLock();
                base.Query(ref range, results);
            }
            finally
            {
                readWriteLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public override void QueryAllInBoundsItems(List<T> results)
        {
            try
            {
                readWriteLock.EnterReadLock();
                base.QueryAllInBoundsItems(results);
            }
            finally
            {
                readWriteLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public override List<T> QueryItemsThatAreOutOfBounds()
        {
            try
            {
                readWriteLock.EnterReadLock();
                return base.QueryItemsThatAreOutOfBounds();
            }
            finally
            {
                readWriteLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public override bool Remove(T item)
        {
            try
            {
                readWriteLock.EnterWriteLock();
                return base.Remove(item);
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public override void ResizeBoundary(Rect newBoundary)
        {
            try
            {
                readWriteLock.EnterWriteLock();
                base.ResizeBoundary(newBoundary);
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }
    }
}