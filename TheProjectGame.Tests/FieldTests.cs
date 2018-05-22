using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using Player;

namespace TheProjectGame.Tests
{
    [TestClass]
    public class TaskFieldTests
    {
        [TestMethod]
        public void NewTaskField()
        {
            var field = new GameArea.GameObjects.TaskField(new TaskField(1, 1));
            Assert.AreEqual(1, field.X);
            Assert.AreEqual(2, field.Y);
            Assert.IsNull(field.Piece);
        }

        [TestMethod]
        public void NewTaskFiledWithPiece()
        {
            var piece = new GameArea.GameObjects.Piece(new Piece());
            var fieldWithPiece = new GameArea.GameObjects.TaskField(new TaskField(1, 2));
            fieldWithPiece.Piece = piece;
            Assert.IsNotNull(fieldWithPiece.Piece);
        }

        [TestMethod]
        public void NewTaskAddPiece()
        {
            var piece = new GameArea.GameObjects.Piece(new Piece());
            var emptyField = new GameArea.GameObjects.TaskField(new TaskField(1, 2));
            emptyField.Piece = piece;
            Assert.IsNotNull(emptyField.Piece);
        }

        [TestMethod]
        public void TaskFieldGetType()
        {
            var field = new GameArea.GameObjects.TaskField(new TaskField(1, 1));
            Assert.AreEqual(GameArea.GameObjects.FieldType.Task, field.GetFieldType);
        }
    }

    [TestClass]
    public class GoalFieldTests
    {
        [TestMethod]
        public void NewGoalField()
        {
            var field = new GameArea.GameObjects.GoalField(new GoalField(1, 2, TeamColour.blue));
            Assert.AreEqual(1, field.X);
            Assert.AreEqual(2, field.Y);
        }

        [TestMethod]
        public void GoalFieldGetType()
        {
            var field = new GameArea.GameObjects.GoalField(new GoalField(1, 1, TeamColour.blue));
            Assert.AreEqual(GameArea.GameObjects.FieldType.Goal, field.GetFieldType);
        }

        [TestMethod]
        public void NewGoalType()
        {
            var field = new GameArea.GameObjects.GoalField(new GoalField(1, 1, TeamColour.blue));
            Assert.AreEqual(GoalFieldType.unknown, field.Type);
        }

        [TestMethod]
        public void NewGoalTypeGoal()
        {
            var field = new GameArea.GameObjects.GoalField(new GoalField(1, 1, TeamColour.blue, GoalFieldType.goal));
            Assert.AreEqual(GoalFieldType.goal, field.Type);
        }
    }


}
