using System;
using System.Collections.Generic;
using System.Linq;

namespace magicmanam.UndoRedo
{
    public class UndoableContext<T> : IUndoableContext<T> where T : class
    {
        public event EventHandler<UndoableActionEventArgs<T>> UndoableAction;

        private readonly Stack<UndoableAction<T>> _actions = new Stack<UndoableAction<T>>();
        private readonly Stack<UndoableAction<T>> _undoActions = new Stack<UndoableAction<T>>();
        private readonly Stack<UndoableAction<T>> _nestedActionsStack = new Stack<UndoableAction<T>>();
        private IStatefulComponent<T> _stateKeeper;

        public UndoableContext(IStatefulComponent<T> stateKeeper)
        {
            this._stateKeeper = stateKeeper;
        }

        public static IUndoableContext<T> Current { get; set; }

        public UndoableAction<T> StartAction(string actionName = "")
        {
            var action = new UndoableAction<T>(actionName, this._stateKeeper.UndoableState, this._nestedActionsStack.Any());
            action.ActionEnd += OnActionEnd;

            this._nestedActionsStack.Push(action);

            return action;
        }

        private void OnActionEnd(object sender, UndoableActionEventArgs<T> e)
        {
            var action = this._nestedActionsStack.Pop();

            if (action.ActionId != e.Action.ActionId)
            {
                throw new InvalidOperationException("Nested actions must be disposed in stack order.");
            }

            if (action.IsCancelled)
            {
                this._stateKeeper.UndoableState = action.StateOnStart;
            }

            if (!this.IsOperationInProgress)
            {
                action.StateOnEnd = this._stateKeeper.UndoableState;
                this._actions.Push(action);

                this._undoActions.Clear();
                e.CanUndo = true;
                e.CanRedo = false;
                this.UndoableAction?.Invoke(this, e);
            }
        }

        public bool IsOperationInProgress
        {
            get
            {
                return this._nestedActionsStack.Any();
            }
        }

        public void Undo()
        {
            if (this.IsOperationInProgress)
            {
                throw new InvalidOperationException($"Operation is in progress. Check {nameof(IsOperationInProgress)} property before calling this method");
            }

            if (this._actions.Count > 0)
            {
                var lastAction = this._actions.Pop();

                this._undoActions.Push(lastAction);

                this._stateKeeper.UndoableState = lastAction.StateOnStart;
                this.UndoableAction?.Invoke(this, new UndoableActionEventArgs<T>(lastAction, true, false) { CanUndo = this._undoActions.Any(), CanRedo = true });
            }
        }
        public void Redo()
        {
            if (this.IsOperationInProgress)
            {
                throw new InvalidOperationException($"Operation is in progress. Check {nameof(IsOperationInProgress)} property before calling this method");
            }

            if (this._undoActions.Count > 0)
            {
                var undoAction = this._undoActions.Pop();

                this._actions.Push(undoAction);

                this._stateKeeper.UndoableState = undoAction.StateOnEnd;
                this.UndoableAction?.Invoke(this, new UndoableActionEventArgs<T>(undoAction, false, true) { CanUndo = true, CanRedo = this._undoActions.Any() });
            }
        }
    }
}