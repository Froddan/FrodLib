using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{

    /// <summary>
    /// Used to wrap undo redo changes into an transaction
    /// </summary>
    public class UndoRedoTransaction : IDisposable
    {
        private UndoRedoManager m_manager;

        public UndoRedoTransaction(UndoRedoManager manager) : this(manager, string.Empty)
        {
        }

        public UndoRedoTransaction(UndoRedoManager manager, string transactionName)
        {
            this.m_manager = manager;
            TransactionName = transactionName;
            manager.SetCurrentTransaction(this);
        }

        public void Dispose()
        {
            EndTransaction();
        }

        public void EndTransaction()
        {
            m_manager.EndCurrentTransaction();
        }

        public string TransactionName { get; private set; }
    }
}
