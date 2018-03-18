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
            var field = new GameArea.TaskField(1u, 2u);
            Assert.AreEqual(1u, field.x);
            Assert.AreEqual(2u, field.y);
            Assert.IsNull(field.GetPiece);
        }

        [TestMethod]
        public void NewTaskFiledWithPiece()
        {
            var piece = new Piece();
            var fieldWithPiece = new GameArea.TaskField(1, 2, piece);
            Assert.IsNotNull(fieldWithPiece.GetPiece);
        }

        [TestMethod]
        public void NewTaskAddPiece()
        {
            var piece = new Piece();
            var emptyField = new GameArea.TaskField(1, 2);
            emptyField.SetPiece(piece);
            Assert.IsNotNull(emptyField.GetPiece);
        }

        [TestMethod]
        public void TaskRemovePiece()
        {
            var piece = new Piece();
            var taskField = new GameArea.TaskField(1, 2, piece);
            taskField.RemovePiece();
            Assert.IsNull(taskField.GetPiece);
        }

        [TestMethod]
        public void TaskFieldGetType()
        {
            var field = new GameArea.TaskField(1, 1);
            Assert.AreEqual(FieldType.Task, field.GetFieldType());
        }
    }

    [TestClass]
    public class GoalFieldTests
    {
        [TestMethod]
        public void NewGoalField()
        {
            var field = new GameArea.GoalField(1u, 2u, TeamColour.blue);
            Assert.AreEqual(1u, field.x);
            Assert.AreEqual(2u, field.y);
            Assert.AreEqual(TeamColour.blue, field.Owner);
        }

        [TestMethod]
        public void GoalFieldGetType()
        {
            var field = new GameArea.GoalField(1u, 1u, TeamColour.blue);
            Assert.AreEqual(FieldType.Goal, field.GetFieldType());
        }

        [TestMethod]
        public void NewGoalType()
        {
            var field = new GameArea.GoalField(1, 1, TeamColour.blue);
            Assert.AreEqual(GoalFieldType.unknown, field.GoalType);
        }

        [TestMethod]
        public void NewGoalTypeGoal()
        {
            var field = new GameArea.GoalField(1, 1, TeamColour.blue, GoalFieldType.goal);
            Assert.AreEqual(GoalFieldType.goal, field.GoalType);
        }
    }


}
