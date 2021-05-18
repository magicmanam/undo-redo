using System;

namespace magicmanam.UndoRedo
{
    public class UndoableAction<T> : IDisposable where T : class
    {
        internal event EventHandler<UndoableActionEventArgs<T>> ActionEnd;

        internal UndoableAction(string name, T state, bool isNested)
        {
            this.Name = name;
            this.StateOnStart = state;
            this.IsNested = isNested;
            this.ActionId = Guid.NewGuid();
            this.AtTimeUtc = DateTime.UtcNow;
        }

        internal T StateOnStart { get; private set; }
        internal T StateOnEnd { get; set; }

        public bool IsNested { get; set; }

        public string Name { get; private set; }

        public bool IsCancelled { get; private set; }

        public void Cancel() {
            this.IsCancelled = true;
        }

        public Guid ActionId { get; private set; }
        public DateTime AtTimeUtc { get; private set; }

        public void Dispose()
        {
            this.ActionEnd?.Invoke(this, new UndoableActionEventArgs<T>(this));
        }
    }
}
