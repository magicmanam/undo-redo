using System;

namespace magicmanam.UndoRedo
{
    public interface IUndoableContext<T> where T : class
    {
        event EventHandler<UndoableActionEventArgs<T>> UndoableAction;

        UndoableAction<T> StartAction(string action = "");

        void Undo();
        void Redo();
    }
}
