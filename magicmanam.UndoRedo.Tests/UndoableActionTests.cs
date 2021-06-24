using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace magicmanam.UndoRedo.Tests
{
    [TestClass]
    public class UndoableActionTests
    {
        [TestMethod]
        public void Cancel_sets_is_cancelled_property()
        {
            // Arrange
            var action = new UndoableAction<List<int>>("name", null, false);

            // Act
            action.Cancel();

            // Assert
            Assert.IsTrue(action.IsCancelled);
        }

        [TestMethod]
        public void On_creation_sets_current_UTC_time()
        {
            // Arrange
            var utcBeforeCreation = DateTime.UtcNow;

            // Act
            var action = new UndoableAction<List<int>>("action", null, false);

            // Assert
            Assert.IsTrue(DateTime.UtcNow >= action.AtTimeUtc);
            Assert.IsTrue(action.AtTimeUtc >= utcBeforeCreation);
        }

        [TestMethod]
        public void On_creation_sets_current_UTC_tim1e()
        {
            // Arrange
            var action = new UndoableAction<List<int>>("action", null, false);
            action.ActionEnd += (object sender, UndoableActionEventArgs<List<int>> e) =>
            {
                // Assert
                Assert.AreEqual(action, sender);
                Assert.IsFalse(e.IsRedo);
                Assert.IsFalse(e.IsUndo);
            };

            // Act
            action.Dispose();
        }
    }
}
