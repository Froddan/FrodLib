using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FrodLib.Utils
{
    public static class COMDisposer
    {

        public static void DisposeCOMComponent<T>(ref T comObject)
        {
            if (comObject == null) return;
            if (Marshal.IsComObject(comObject))
            {
                Marshal.ReleaseComObject(comObject);
                comObject = default(T);
            }
        }

    }
}
