namespace magicmanam.UndoRedo
{
    public interface IStatefulComponent<T>
    {
        /// <summary>
        /// Returns deep copy of undoable component's state.
        /// </summary>
        T UndoableState { get; set; }
    }
}
