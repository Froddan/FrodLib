using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{
    public static class Swapper
    {
        public static void Swap<T>(ref T item1, ref T item2)
        {
            T tmp = item1;
            item1 = item2;
            item2 = tmp;
        }
    }
}
