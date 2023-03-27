using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.UI.Common.Classes
{
    public sealed class OverrideCursor : IDisposable
    {
        private static Stack<Cursor> _cursors = new Stack<Cursor>();

        public OverrideCursor(Cursor changeToCursor)
        {
            _cursors.Push(changeToCursor);

            if (Mouse.OverrideCursor != changeToCursor)
                Mouse.OverrideCursor = changeToCursor;
        }

        public void Dispose()
        {
            _cursors.Pop();

            var cursor = _cursors.Count > 0 ? _cursors.Peek() : null;

            if (cursor != Mouse.OverrideCursor)
                Mouse.OverrideCursor = cursor;
        }
    }
}
