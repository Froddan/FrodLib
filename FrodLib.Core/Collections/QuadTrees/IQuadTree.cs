using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Collections.QuadTrees
{
    public interface IQuadTree<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        Rect Boundary { get; }
        bool Insert(T item, Rect itemSizeAndPos);
        List<T> Query(Point point);
        List<T> Query(Rect range);
        bool MoveItem(T item, Rect newItemBoundary);
        void ResizeBoundary(Rect newBoundary);
    }

    public interface IBoundary<T>
    {
        Rect Boundary { get; }
        T Implementation { get; }
    }
}
