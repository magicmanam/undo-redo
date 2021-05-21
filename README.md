magicmanam.UndoRedo
==============================

`magicmanam.UndoRedo` [nuget package](https://www.nuget.org/packages/magicmanam.UndoRedo) is easy and lightweight Undo/Redo operations implementation

In order to add it to your solution, run `Install-Package magicmanam.UndoRedo` from your NuGet Package Manager console in Visual Studio.

### Quick start
1) Define class which will represent your application state at any time (for example **AppState** class)
2) Implement interface <i>IStatefulComponent&lt;AppState&gt;</i> to have possibility to retrieve current application state
3) Initialize current undoable context for your application state:
```csharp
UndoableContext<AppState>.Current = new UndoableContext<AppState>(statefulComponentInstance);
```
4) Wrap any action which changes application state with the following code:
```csharp
using (var action = UndoableContext<AppState>.Current.StartAction("Sample action name"))
{
	statefulComponentInstance.ChangeState();
	// action.Cancel(); - you can cancel your application state change and the previous state will be restored
}
```
5) Now you can undo/redo any your action made in scope of current undoable context:
```csharp
UndoableContext<AppState>.Current.Undo();
// statefulComponentInstance returns the previous state now
// or
UndoableContext<AppState>.Current.Redo();
// Rollbacks the previous Undo operation
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
};
```
