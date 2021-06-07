using magicmanam.UndoRedo;

namespace Sample
{
    public class AppStateComponent : IStatefulComponent<AppState>
    {
        public AppState State { get; set; }

        public AppState UndoableState
        {
            get
            {
                return new AppState
                {
                    X = State.X,
                    Y = State.Y
                };// Deep copy
            }
            set
            {
                this.State.X = value.X;
                this.State.Y = value.Y;
            }
        }
    }
}
