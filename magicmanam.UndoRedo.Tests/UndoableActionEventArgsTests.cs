using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace magicmanam.UndoRedo.Tests
{
    [TestClass]
    public class UndoableActionEventArgsTests
    {
        [TestMethod]
        public void When_action_is_null_throws_ArgumentNullException()
        {
            // Arrange
            UndoableAction<List<int>> action = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new UndoableActionEventArgs<List<int>>(action, false, false));
        }

        [TestMethod]
        public void On_creation_when_action_is_not_null_Action_property_contains_it()
        {
            // Arrange
            UndoableAction<List<int>> action = new UndoableAction<List<int>>("Action", null, false);

            // Act
            var args = new UndoableActionEventArgs<List<int>>(action, false, false);

            // Assert
            Assert.AreEqual(action, args.Action);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void On_creation_sets_IsUndo_property(bool isUndo)
        {
            // Arrange
            UndoableAction<List<int>> action = new UndoableAction<List<int>>("Action", null, false);

            // Act
            var args = new UndoableActionEventArgs<List<int>>(action, isUndo, false);

            // Assert
            Assert.AreEqual(isUndo, args.IsUndo);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void On_creation_sets_IsRedo_property(bool isRedo)
        {
            // Arrange
            UndoableAction<List<int>> action = new UndoableAction<List<int>>("Action", null, false);

            // Act
            var args = new UndoableActionEventArgs<List<int>>(action, false, isRedo);

            // Assert
            Assert.AreEqual(isRedo, args.IsRedo);
        }
    }
}
