using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TheProjectGame.GameArea;

namespace TheProjectGame.Tests
{
    [TestClass]
    public class TaskFieldTests
    {
        [TestMethod]
        public void NewTaskField()
        {
            var field = new TaskField(1, 2);
            Assert.AreEqual(1, field.X);
            Assert.AreEqual(2, field.Y);
            Assert.IsNull(field.GetPiece());
        }

        [TestMethod]
        public void NewTaskFiledWithPiece()
        {
            var piece = new Piece();
            var fieldWithPiece = new TaskField(1, 2, piece);
            Assert.IsNotNull(fieldWithPiece.GetPiece());
        }

        [TestMethod]
        public void NewTaskAddPiece()
        {
            var piece = new Piece();
            var emptyField = new TaskField(1, 2);
            emptyField.SetPiece(piece);
            Assert.IsNotNull(emptyField.GetPiece());
        }

        [TestMethod]
        public void TaskRemovePiece()
        {
            var piece = new Piece();
            var taskField = new TaskField(1, 2, piece);
            taskField.RemovePiece();
            Assert.IsNull(taskField.GetPiece());
        }

        [TestMethod]
        public void TaskFieldGetType()
        {
            var field = new TaskField(1, 1);
            Assert.AreEqual(FieldType.Task, field.GetFieldType());
        }
    }

    [TestClass]
    public class GoalFieldTests
    {
        [TestMethod]
        public void NewGoalField()
        {
            var field = new GoalField(1, 2);
            Assert.AreEqual(1, field.X);
            Assert.AreEqual(2, field.Y);
        }

        [TestMethod]
        public void GoalFieldGetType()
        {
            var field = new GoalField(1, 1);
            Assert.AreEqual(FieldType.Goal, field.GetFieldType());
        }

        [TestMethod]
        public void NewGoalType()
        {
            var field = new GoalField(1, 1);
            Assert.AreEqual(GoalType.Unknown, field.GoalType);
        }

        [TestMethod]
        public void NewGoalTypeNotFullfilled()
        {
            var field = new GoalField(1, 1,GoalType.NotFullfilled);
            Assert.AreEqual(GoalType.NotFullfilled, field.GoalType);
        }
    }
}
