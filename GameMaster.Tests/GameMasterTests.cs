using Configuration;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GameArea.Tests
{
    [TestClass]
    public class GameMasterTests
    {
        GameMasterSettingsGameDefinition defaultSettings = GameMasterSettingsGameDefinition.GetDefaultGameDefinition();
        GameMaster defaultGameMaster = new GameMaster(GameMasterSettings.GetDefaultGameMasterSettings());
        [TestMethod]
        public void InitBoard()
        {
            var taskFields = defaultGameMaster.GetBoard.TaskFields;
            Assert.AreEqual(defaultSettings.InitialNumberOfPieces, (uint)taskFields.Where(q => q.GetPiece != null).Count());
            var ids = new List<ulong>();
            foreach (var piece in taskFields.Where(q=>q.GetPiece!= null).Select(q=> q.GetPiece))
            {
                Assert.IsFalse(ids.Contains(piece.id));
                ids.Add(piece.id);
            }
        }

        [TestMethod]
        public void InitAgentsList()
        {
            Assert.IsNotNull(defaultGameMaster.GetAgents);
            Assert.AreEqual(0, defaultGameMaster.GetAgents.Count);
        }

        [TestMethod]
        public void GameMasterAfterInitState()
        {
            Assert.AreEqual(GameMasterState.AwaitingPlayers, defaultGameMaster.GetState);
        }

        [TestMethod]
        public void GameMasterInitialGoals()
        {
            var blueGoalFields = defaultGameMaster.GetBoard.GetBlueGoalAreaFields.Where(q => q.GoalType == GoalFieldType.goal).ToList();
            var blueGoals = defaultGameMaster.GetGameDefinition.Goals.Where(q => q.team == TeamColour.blue).ToList();
            Assert.AreEqual(blueGoals.Count, blueGoalFields.Count);
            foreach(var blueGoal in blueGoalFields)
            {
                Assert.IsTrue(blueGoals.Where(q => q.x == blueGoal.x && q.y == blueGoal.y && q.type == GoalFieldType.goal && q.team ==TeamColour.blue).Any());
            }


            var redGoalFields = defaultGameMaster.GetBoard.GetRedGoalAreaFields.Where(q => q.GoalType == GoalFieldType.goal).ToList();
            var redGoals = defaultGameMaster.GetGameDefinition.Goals.Where(q => q.team == TeamColour.red).ToList();
            Assert.AreEqual(redGoals.Count, redGoals.Count);
            foreach (var redGoal in redGoalFields)
            {
                Assert.IsTrue(redGoals.Where(q => q.x == redGoal.x && q.y == redGoal.y && q.type == GoalFieldType.goal && q.team == TeamColour.red).Any());
            }
        }

        [TestMethod]
        public void ManhattanDistance()
        {
            GameMasterSettingsGameDefinition settings = GameMasterSettingsGameDefinition.GetDefaultGameDefinition();
            settings.InitialNumberOfPieces = 0;
            GameMaster defaultGameMaster = new GameMaster(GameMasterSettings.GetGameMasterSettings(settings));

            //first piece
            defaultGameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.sham, 97);
            defaultGameMaster.UpdateDistancesFromAllPieces();
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(3, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(2, 4) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).Distance);
            Assert.AreEqual(3, (defaultGameMaster.GetBoard.GetField(4, 6) as GameArea.TaskField).Distance);
            Assert.AreEqual(4, (defaultGameMaster.GetBoard.GetField(4, 3) as GameArea.TaskField).Distance);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(0, 5) as GameArea.TaskField).Distance);

            //second piece
            defaultGameMaster.SetPieceInLocation(4, 5, TeamColour.blue, PieceType.sham, 98);
            defaultGameMaster.UpdateDistancesFromAllPieces();
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(3, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(4, 6) as GameArea.TaskField).Distance);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(4, 3) as GameArea.TaskField).Distance);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(0, 5) as GameArea.TaskField).Distance);

            //third piece
            defaultGameMaster.SetPieceInLocation(1, 8, TeamColour.blue, PieceType.sham, 99);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.TaskField).GetPiece);
            Assert.IsNotNull((defaultGameMaster.GetBoard.GetField(1, 8) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(0, (defaultGameMaster.GetBoard.GetField(4, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(3, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(4, 6) as GameArea.TaskField).Distance);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(4, 3) as GameArea.TaskField).Distance);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(0, 5) as GameArea.TaskField).Distance);
            Assert.AreEqual(1, (defaultGameMaster.GetBoard.GetField(1, 7) as GameArea.TaskField).Distance);
            Assert.AreEqual(2, (defaultGameMaster.GetBoard.GetField(3, 8) as GameArea.TaskField).Distance);
        }
    }
}
