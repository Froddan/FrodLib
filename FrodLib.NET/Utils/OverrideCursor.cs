using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FrodLib.Utils
{
    /// <summary>
    /// Overrides the current cursor.
    /// </summary>
    /// <remarks>Author: Fredrik Schmidt</remarks>
    /// <remarks>Date: 2011-11-17</remarks>
    /// <remarks>Revision History:</remarks>
    /// <review></review>
    [ExcludeFromCodeCoverage]
    public sealed class OverrideCursor : IDisposable
    {
        static Stack<Cursor> s_Stack = new Stack<Cursor>();

        public OverrideCursor(Cursor changeToCursor)
        {
            s_Stack.Push(changeToCursor);

            if (Mouse.OverrideCursor != changeToCursor)
                Mouse.OverrideCursor = changeToCursor;
        }

        /// <summary>
        /// Call dispose to remove the last added cursor
        /// </summary>
        public void Dispose()
        {
            s_Stack.Pop();

            Cursor cursor = s_Stack.Count > 0 ? s_Stack.Peek() : null;

            if (cursor != Mouse.OverrideCursor)
                Mouse.OverrideCursor = cursor;
        }

    }
}
