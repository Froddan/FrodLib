using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{
    public static class Disposer
    {
        public static void TryDisposeObject(this object objToDispose)
        {
            if (objToDispose == null) return;
            var disposable = objToDispose as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
