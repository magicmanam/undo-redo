using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace magicmanam.UndoRedo.Tests
{
    [TestClass]
    public class UndoableContextTests
    {
        private readonly StatefulComponent _component;
        private readonly UndoableContext<List<int>> _undoableContext;
        private readonly List<string> _actionsList = new List<string>();

        public UndoableContextTests()
        {
            this._component = new StatefulComponent();
            this._undoableContext = new UndoableContext<List<int>>(this._component);
        }

        [TestMethod]
        public void Stores_changed_in_action_state()
        {
            using (var action = this._undoableContext.StartAction())
            {
                this._component.A = 1;
            }// Act

            // Assert
            Assert.AreEqual(1, this._component.A);
        }

        [TestMethod]
        public void Undos_to_previous_state()
        {
            using (var action = this._undoableContext.StartAction())
            {
                this._component.A = 1;
            }

            // Act
            this._undoableContext.Undo();

            // Assert
            Assert.AreEqual(0, this._component.A);
        }

        [TestMethod]
        public void Redos_previously_undoed_state()
        {
            using (var action = this._undoableContext.StartAction())
            {
                this._component.A = 1;
            }

            this._undoableContext.Undo();

            // Act
            this._undoableContext.Redo();

            // Assert
            Assert.AreEqual(1, this._component.A);
        }

        [TestMethod]
        public void When_nested_actions_and_second_is_cancelled_stores_first_action_state()
        {
            using (var action1 = this._undoableContext.StartAction())
            {
                this._component.A = 1;

                using (var action2 = this._undoableContext.StartAction())
                {
                    this._component.A = 2;
                    action2.Cancel();
                }// Act
            }

            // Assert
            Assert.AreEqual(1, this._component.A);
        }

        [TestMethod]
        public void When_nested_actions_and_first_is_cancelled_stores_second_action_state()
        {
            using (var action1 = this._undoableContext.StartAction())
            {
                using (var action2 = this._undoableContext.StartAction())
                {
                    this._component.A = 2;
                    action2.Cancel();
                }// Act

                this._component.A = 1;
            }

            // Assert
            Assert.AreEqual(1, this._component.A);
        }

        [TestMethod]
        public void When_nested_actions_stores_final_state()
        {
            this._undoableContext.UndoableAction += this._UndoableAction;

            using (var action = this._undoableContext.StartAction("Outer"))
            {
                this._component.A = 1;

                using (var action1 = this._undoableContext.StartAction("Nested"))
                {
                    this._component.A = 2 * this._component.A;
                }

                using (var action2 = this._undoableContext.StartAction("Nested"))
                {
                    this._component.A = 2 * this._component.A;
                }

                this._component.A = 2 * this._component.A;
            }

            // Assert
            Assert.AreEqual(8, this._component.A);
            Assert.AreEqual("Outer", this._actionsList.Single());
        }

        [TestMethod]
        public void When_no_actions_to_undo_event_arguments_states_that_could_not_undo_more()
        {
            // Arrange
            using (var action1 = this._undoableContext.StartAction())
            {
                this._component.A = 1;
            }

            using (var action2 = this._undoableContext.StartAction())
            {
                this._component.A = 2;
            }

            var args = new List<UndoableActionEventArgs<List<int>>>();
            this._undoableContext.UndoableAction += (object sender, UndoableActionEventArgs<List<int>> e) =>
            {
                args.Add(e);
            };

            // Act

            this._undoableContext.Undo();
            this._undoableContext.Undo();

            // Assert
            Assert.AreEqual(true, args[0].CanUndo);
            Assert.AreEqual(true, args[0].CanRedo);

            Assert.AreEqual(false, args[1].CanUndo);
            Assert.AreEqual(true, args[1].CanRedo);
        }

        private void _UndoableAction(object sender, UndoableActionEventArgs<List<int>> e)
        {
            this._actionsList.Add(e.Action.Name);
        }
    }
}
