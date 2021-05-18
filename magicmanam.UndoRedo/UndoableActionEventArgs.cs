using System;

namespace magicmanam.UndoRedo
{
    public class UndoableActionEventArgs<T> : EventArgs where T : class
    {
        internal UndoableActionEventArgs(UndoableAction<T> action, bool isUndo = false, bool isRedo = false)
        {
            this.Action = action;
            this.IsUndo = isUndo;
            this.IsRedo = isRedo;
        }

        public UndoableAction<T> Action { get; private set; }

        public bool IsUndo { get; private set; }

        public bool IsRedo { get; private set; }

        public bool CanUndo { get; internal set; }

        public bool CanRedo { get; internal set; }
    }
}
