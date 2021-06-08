magicmanam.UndoRedo
==============================

`magicmanam.UndoRedo` [nuget package](https://www.nuget.org/packages/magicmanam.UndoRedo) is easy and lightweight Undo/Redo operations implementation

In order to add it to your solution, run `Install-Package magicmanam.UndoRedo` from your NuGet Package Manager console in Visual Studio.

### Quick start
1) Define class which will represent your application state at any time (for example **AppState** class)
```csharp
public class AppState
{
    public int X { get; set; }
    public int Y { get; set; }
}
```
2) Implement interface <i>IStatefulComponent&lt;AppState&gt;</i> to have possibility to retrieve current application state:
```csharp
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
```
3) Initialize current undoable context for your application state:
```csharp
var stateComponent = new AppStateComponent();
stateComponent.State = new AppState { X = 0, Y = 0 };

UndoableContext<AppState>.Current = new UndoableContext<AppState>(stateComponent);
```
4) Wrap any action which changes application state with the following code:
```csharp
using (var action = UndoableContext<AppState>.Current.StartAction("Sample action name"))
{
    stateComponent.State.X = 23;
    stateComponent.State.Y = 10;
    // action.Cancel(); - you can cancel your application state change and the previous state will be restored
    // nested actions are supported: there is outer action tracked only, any action can be cancelled.
}
```
5) Now you can undo/redo any your action made in scope of current undoable context:
```csharp
UndoableContext<AppState>.Current.Undo();
// stateComponent returns the previous state now: X = 0, Y = 0
// or
UndoableContext<AppState>.Current.Redo();
// Rollbacks the previous Undo operation: X = 23, Y = 10
```
6) You can subscribe on any action in scope of the current undoable context including cancelled actions and Undo/Redo operations:
```csharp
UndoableContext<AppState>.Current.UndoableAction += (object sender, UndoableActionEventArgs<AppState> e) =>
{
    if (e.Action.IsCancelled)
    {
        // Action was cancelled
    }
    else
    {
        if (e.IsUndo)
        {
             // The latest action was undone
        }
        else if (e.IsRedo)
        {
             // The latest undo operation was restored
        }
        else
        {
             // e.Action.Name == "Sample action name"
             // Ordinary action on application state was performed
        }
    }

    if (e.Action.IsNested)
    {
        // Action is nested
    }
};
```

Icon's source: https://www.shareicon.net/religion-philosophy-taoism-signs-yin-yang-balance-748079
