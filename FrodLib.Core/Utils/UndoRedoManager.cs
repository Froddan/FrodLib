using FrodLib.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FrodLib.Utils
{
    public delegate void UndoRedoOperation<T>(T data);

    public class UndoRedoManager
    {
        private UndoRedoTransactionOperation currentTransActionOperation;
        private UndoRedoTransaction m_currentTransaction;
        private bool m_doingRedo;
        private bool m_doingUndo;
        private int m_maxItems;
        private LinkedList<IUndoRedoOperation> m_redoActions;
        private LinkedList<IUndoRedoOperation> m_undoActions;

        public UndoRedoManager()
        {
            m_undoActions = new LinkedList<IUndoRedoOperation>();
            m_redoActions = new LinkedList<IUndoRedoOperation>();
        }

        public event EventHandler RedoStackStatusChanged;
        public event EventHandler UndoStackStatusChanged;

        private interface IUndoRedoOperation
        {
            void Execute();
        }

        /// <summary>
        /// Returns true if there are any available redo operations
        /// </summary>
        public bool HasRedoOperations
        {
            get
            {
                return m_redoActions.Any();
            }
        }

        /// <summary>
        /// Returns true if there are any available undo operations
        /// </summary>
        public bool HasUndoOperations
        {
            get
            {
                return m_undoActions.Any();
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of items to be stored in stack. To store infinitive number of operations set it to zero
        /// </summary>
        public int MaxItems
        {
            get { return m_maxItems; }
            set
            {
                if (m_maxItems == value) return;
                if (value < 0) throw new ArgumentOutOfRangeException(StringResources.MaxItemsLessThenZero);

                m_maxItems = value;
                MaintainMaxItems();
            }
        }

        /// <summary>
        /// Returns the number of redo operations available in the stack
        /// </summary>
        public int NumberOfRedoOperations
        {
            get
            {
                return m_redoActions.Count;
            }
        }

        /// <summary>
        /// Returns the number of undo operations available in the stack
        /// </summary>
        public int NumberOfUndoOperations
        {
            get
            {
                return m_undoActions.Count;
            }
        }

        /// <summary>
        /// Clears the manager of any available undo and/or redo
        /// </summary>
        public void Clear()
        {
            ClearIfAny(m_undoActions, RaiseUndoStackStatusChanged);
            ClearIfAny(m_redoActions, RaiseRedoStackStatusChanged);
        }

        /// <summary>
        /// Pushes an item onto the undo/redo stack. 
        /// 1) If this is called outside the context of a undo/redo operation, the item is added to the undo stack.
        /// 2) If this is called in the context of an undo operation, the item is added to redo stack.
        /// 3) If this is called in context of an redo operation, item is added to undo stack.
        /// </summary>
        public void Push<T>(UndoRedoOperation<T> undoRedoAction, T undoRedoData)
        {

            if (m_currentTransaction != null)
            {
                if (currentTransActionOperation == null)
                {
                    currentTransActionOperation = new UndoRedoTransactionOperation();
                }
                currentTransActionOperation.AddUndoRedoOperation(new UndoRedoOperationHolder<T>(undoRedoAction, undoRedoData));
            }
            else if (m_doingUndo)
            {
                m_redoActions.AddLast(new UndoRedoOperationHolder<T>(undoRedoAction, undoRedoData));
                RaiseRedoStackStatusChanged();
            }
            else
            {
                if (!m_doingRedo)
                {
                    ClearIfAny(m_redoActions, RaiseRedoStackStatusChanged);
                }
                m_undoActions.AddLast(new UndoRedoOperationHolder<T>(undoRedoAction, undoRedoData));
                RaiseUndoStackStatusChanged();
            }
            MaintainMaxItems();
        }

        /// <summary>
        /// Redoes the last added operation that was undone using undo. Throws invalid operation exception if there exists no redo operation
        /// </summary>
        public void Redo()
        {
            if (!m_redoActions.Any()) throw new InvalidOperationException(StringResources.NoRedoAvailable);
            if (m_currentTransaction != null) throw new UndoRedoTransactionActiveException(StringResources.CannotRedoWhileTransaction);
            try
            {
                m_doingRedo = true;
                IUndoRedoOperation operation = m_redoActions.Last.Value;
                m_redoActions.RemoveLast();

                if (operation is UndoRedoTransactionOperation)
                {
                    using (new UndoRedoTransaction(this))
                    {
                        operation.Execute();
                    }
                }
                else
                {
                    operation.Execute();
                }
            }
            finally
            {
                m_doingRedo = false;
                RaiseRedoStackStatusChanged();
            }
        }

        /// <summary>
        /// Undoes the last added operation. Throws invalid operation exception if there exists no undo operation
        /// </summary>
        public void Undo()
        {
            if (!m_undoActions.Any()) throw new InvalidOperationException(StringResources.NoUndoAvailable);
            if (m_currentTransaction != null) throw new UndoRedoTransactionActiveException(StringResources.CannotUndoWhileTransaction);
            try
            {
                m_doingUndo = true;
                IUndoRedoOperation operation = m_undoActions.Last.Value;
                m_undoActions.RemoveLast();

                if (operation is UndoRedoTransactionOperation)
                {
                    using (new UndoRedoTransaction(this))
                    {
                        operation.Execute();
                    }
                }
                else
                {
                    operation.Execute();
                }
            }
            finally
            {
                m_doingUndo = false;
                RaiseUndoStackStatusChanged();
            }

        }

        internal void EndCurrentTransaction()
        {
            if (currentTransActionOperation != null && currentTransActionOperation.HasOperations)
            {
                if (m_doingUndo)
                {
                    m_redoActions.AddLast(currentTransActionOperation);
                    RaiseRedoStackStatusChanged();
                }
                else
                {
                    m_undoActions.AddLast(currentTransActionOperation);
                    RaiseUndoStackStatusChanged();
                }
            }

            currentTransActionOperation = null;
            m_currentTransaction = null;

        }

        internal void SetCurrentTransaction(UndoRedoTransaction undoRedoTransaction)
        {
            if (m_currentTransaction != null)
            {
                string exceptionMsg;
                if (string.IsNullOrWhiteSpace(m_currentTransaction.TransactionName))
                {
                    exceptionMsg = StringResources.TransactionActive;
                }
                else
                {
                    exceptionMsg = string.Format(StringResources.TransactionNameActive, "'" + m_currentTransaction.TransactionName + "'");
                }
                throw new UndoRedoTransactionActiveException(exceptionMsg);
            }
            m_currentTransaction = undoRedoTransaction;
        }
        protected void RaiseRedoStackStatusChanged()
        {
            var handler = RedoStackStatusChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected void RaiseUndoStackStatusChanged()
        {
            var handler = UndoStackStatusChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void ClearIfAny(LinkedList<IUndoRedoOperation> actions, Action statusChangedAction)
        {
            if (actions.Any())
            {
                actions.Clear();
                statusChangedAction();
            }
        }

        private void MaintainMaxItems()
        {
            if (m_maxItems == 0) return;
            while (m_undoActions.Count > MaxItems)
            {
                m_undoActions.RemoveFirst();
            }
        }
        private class UndoRedoOperationHolder<T> : IUndoRedoOperation
        {
            private UndoRedoOperation<T> m_undoRedoOperation;
            private T m_undoRedoOperationData;

            public UndoRedoOperationHolder(UndoRedoOperation<T> undoRedoOperation, T undoRedoOperationData)
            {
                m_undoRedoOperation = undoRedoOperation;
                m_undoRedoOperationData = undoRedoOperationData;
            }

            public void Execute()
            {
                m_undoRedoOperation(m_undoRedoOperationData);
            }
        }

        private class UndoRedoTransactionOperation : IUndoRedoOperation
        {
            private List<IUndoRedoOperation> transactionOperations = new List<IUndoRedoOperation>();

            public bool HasOperations
            {
                get
                {
                    return transactionOperations.Any();
                }
            }

            public void AddUndoRedoOperation(IUndoRedoOperation operation)
            {
                transactionOperations.Add(operation);
            }

            public void Execute()
            {
                foreach (var operation in transactionOperations)
                {
                    operation.Execute();
                }
            }
        }
    }


    public class UndoRedoTransactionActiveException : Exception
    {
        public UndoRedoTransactionActiveException()
            : base()
        {

        }

        public UndoRedoTransactionActiveException(string message)
            : base(message)
        {

        }

        public UndoRedoTransactionActiveException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

    }
}
