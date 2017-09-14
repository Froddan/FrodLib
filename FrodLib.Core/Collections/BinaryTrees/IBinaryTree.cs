using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Collections.BinaryTrees
{
    public interface IBinaryTree<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        /// <summary>
        /// Inserts the item into the <see cref=""/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Insert(T item);
        T Find(T item);
    }
}
