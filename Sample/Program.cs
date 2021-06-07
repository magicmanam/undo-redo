using magicmanam.UndoRedo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var stateComponent = new AppStateComponent();
            stateComponent.State = new AppState { X = 0, Y = 0 };

            UndoableContext<AppState>.Current = new UndoableContext<AppState>(stateComponent);

            UndoableContext<AppState>.Current.UndoableAction += (object sender, UndoableActionEventArgs<AppState> e) =>
            {
                if (e.Action.IsCancelled)
                {
                    Console.WriteLine($"Action with name \"{e.Action.Name}\" was cancelled");
                }
                else
                {
                    if (e.IsUndo)
                    {
                        Console.WriteLine($"The latest action was undone (\"{e.Action.Name}\")");
                    }
                    else if (e.IsRedo)
                    {
                        Console.WriteLine($"The latest undo operation was restored (\"{e.Action.Name}\")");
                    }
                    else
                    {
                        Console.WriteLine($"Action name: \"{e.Action.Name}\"");
                    }
                    Console.WriteLine($"{(e.CanUndo ? "Can undo" : "Can not undo")}, {(e.CanRedo ? "can redo" : "can not redo")}. {(e.Action.IsNested ? "Nested action." : "Not nested action")}");
                }

                Console.WriteLine();
            };

            using (var action = UndoableContext<AppState>.Current.StartAction("Sample action name"))
            {
                stateComponent.State.X = 23;
                stateComponent.State.Y = 10;
                // action.Cancel(); - you can cancel your application state change and the previous state will be restored
                // nested actions are supported: there is outer action tracked only, any action can be cancelled.
            }

            UndoableContext<AppState>.Current.Undo();
            // stateComponent returns the previous state now: X = 0, Y = 0
            // or
            UndoableContext<AppState>.Current.Redo();
            // Rollbacks the previous Undo operation: X = 23, Y = 10

            if (stateComponent.State.X != 23 || stateComponent.State.Y != 10)
            {
                throw new ApplicationException("Sample is wrong");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
