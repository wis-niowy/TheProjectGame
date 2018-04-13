using Configuration;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameArea.Tests
{
    [TestClass]
    public class GameMasterTests
    {
        //GameMasterSettingsGameDefinition defaultSettings = GameMasterSettingsGameDefinition.GetDefaultGameDefinition();
        //GameMaster defaultGameMaster = new GameMaster(GameMasterSettings.GetDefaultGameMasterSettings());

        GameMasterSettings defaultSettings = GameMasterSettings.GetDefaultGameMasterSettings();
        GameMaster defaultGameMaster = new GameArea.GameMaster(GameMasterSettings.GetDefaultGameMasterSettings());

        [TestMethod]
        public void InitBoard()
        {
            var taskFields = defaultGameMaster.GetBoard.TaskFields;
            Assert.AreEqual(defaultSettings.GameDefinition.InitialNumberOfPieces, taskFields.Where(q => q.GetPiece != null).Count());
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

        [TestMethod]
        public void GameMasterPlacesShamPieceOnGoalField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0001");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 70)
            {
                timestamp = DateTime.Now,

            });
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            agent.SetPiece(new Piece(PieceType.unknown, 70)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 2); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(1, 2, "testGUID-0001"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = defaultGameMaster.TryPlaceShamPieceOnGoalField(new Location(1,2), "testGUID-0001");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(1, actionResult[0].x);
            Assert.AreEqual(2, actionResult[0].y);
            Assert.AreEqual(agent.ID, actionResult[0].playerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetGoalField(1,2).HasAgent());
            Assert.AreEqual(GoalFieldType.unknown, actionResult[0].type);
        }

        [TestMethod]
        public void GameMasterPlacesNormalPieceOnGoalFieldOfTypeGoal()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0001");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 70)
            {
                timestamp = DateTime.Now,

            });
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            agent.SetPiece(new Piece(PieceType.unknown, 70)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 1); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(1, 1, "testGUID-0001"); // we change a location of GM's copy
            var expectedScore = defaultGameMaster.GoalsBlueLeft - 1;

            // action: agent places a piece
            var actionResult = defaultGameMaster.TryPlaceNormalPieceOnGoalField(new Location(1, 1), "testGUID-0001");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(1, actionResult[0].x);
            Assert.AreEqual(1, actionResult[0].y);
            Assert.AreEqual(agent.ID, actionResult[0].playerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetGoalField(1, 1).HasAgent());
            Assert.AreEqual(GoalFieldType.goal, actionResult[0].type);
            Assert.AreEqual(expectedScore, defaultGameMaster.GoalsBlueLeft);
        }

        [TestMethod]
        public void GameMasterPlacesNormalPieceOnGoalFieldOfTypeNonGoal()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0001");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 70)
            {
                timestamp = DateTime.Now,

            });
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            agent.SetPiece(new Piece(PieceType.unknown, 70)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(2, 1); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(2, 1, "testGUID-0001"); // we change a location of GM's copy
            var expectedScore = defaultGameMaster.GoalsBlueLeft;

            // action: agent places a piece
            var actionResult = defaultGameMaster.TryPlaceNormalPieceOnGoalField(new Location(2, 1), "testGUID-0001");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(2, actionResult[0].x);
            Assert.AreEqual(1, actionResult[0].y);
            Assert.AreEqual(agent.ID, actionResult[0].playerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetGoalField(2, 1).HasAgent());
            Assert.AreEqual(GoalFieldType.nongoal, actionResult[0].type);
            Assert.AreEqual(expectedScore, defaultGameMaster.GoalsBlueLeft);
        }

        [TestMethod]
        public void GameMasterPlacesShamPieceOnNotOccupiedTaskField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0003");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,

            });
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            agent.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 5); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0003"); // we change a location of GM's copy
            // assure there is no piece on the field
            defaultGameMaster.GetBoard.GetTaskField(1, 5).SetPiece(null);

            // action: agent places a piece
            var actionResult = defaultGameMaster.TryPlacePieceOnTaskField(new Location(1, 5), "testGUID-0003");

            var agentGameMasterCopy = defaultGameMaster.GetAgents.Where(q => q.GUID == "testGUID-0003").First();

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(1, actionResult[0].x);
            Assert.AreEqual(5, actionResult[0].y);
            Assert.AreEqual(agent.ID, actionResult[0].playerId);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetTaskField(1, 5).HasAgent());
            Assert.IsNotNull(defaultGameMaster.GetBoard.GetTaskField(1, 5).GetPiece);
            Assert.AreEqual(90ul, defaultGameMaster.GetBoard.GetTaskField(1, 5).GetPiece.id);
            Assert.IsNull(agentGameMasterCopy.GetPiece);
        }

        [TestMethod]
        public void GameMasterPlacesShamPieceOnOccupiedTaskField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0003");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,

            });
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            agent.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 5); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0003"); // we change a location of GM's copy
            // assure there is a piece on the field
            defaultGameMaster.GetBoard.GetTaskField(1, 5).SetPiece(new Piece(PieceType.unknown, 100));

            // action: agent places a piece
            var actionResult = defaultGameMaster.TryPlacePieceOnTaskField(new Location(1, 5), "testGUID-0003");

            var agentGameMasterCopy = defaultGameMaster.GetAgents.Where(q => q.GUID == "testGUID-0003").First();

            Assert.AreEqual(true, setPositionResult);
            Assert.IsNull(actionResult[0]);
            Assert.IsTrue(defaultGameMaster.GetBoard.GetTaskField(1, 5).HasAgent());
            Assert.IsNotNull(defaultGameMaster.GetBoard.GetTaskField(1, 5).GetPiece);
            Assert.AreEqual(100ul, defaultGameMaster.GetBoard.GetTaskField(1, 5).GetPiece.id);
            Assert.IsNotNull(agentGameMasterCopy.GetPiece);
        } 

        [TestMethod]
        public void GameMasterPerformsMoveActionToTaskField()
        {
            var currentLocation = new Location(2, 4);
            var futureLocation = new Location(2, 5);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0013");
            agent.SetLocation(2, 4);

            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);

            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(2, 4, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 4).Player);

            // action: agent moves up to (2,5)
            Data response = new Data()
            {
                playerId = agent.ID,
                TaskFields = new Messages.TaskField[] { },
                GoalFields = null,
                PlayerLocation = currentLocation
            };
            Messages.Piece piece;
            Messages.Field field;
            defaultGameMaster.PerformMoveAction(currentLocation, futureLocation, "testGUID-0013", response);

            var agentGameMasterCopy = defaultGameMaster.GetAgentByGuid("testGUID-0013");

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(FieldType.Task, defaultGameMaster.GetBoard.GetField(2, 4).GetFieldType);
            Assert.AreEqual(FieldType.Task, defaultGameMaster.GetBoard.GetField(2, 5).GetFieldType);
            Assert.AreEqual(futureLocation, agentGameMasterCopy.GetLocation);
            Assert.IsNull(defaultGameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(agent.ID, defaultGameMaster.GetBoard.GetField(2, 5).Player.id);
            Assert.AreEqual(futureLocation, response.PlayerLocation);
        }

        [TestMethod]
        public void GameMasterSetsInfoAboutDiscoveredTaskField()
        {
            List<Messages.TaskField> list = new List<Messages.TaskField>();
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0013");
            var field = defaultGameMaster.GetBoard.GetTaskField(2, 5);
            var piece = new Piece(PieceType.normal, 10);
            field.SetPiece(piece);
            agent.SetLocation(2, 5);
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(2, 5, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(true, setPositionResult);

            defaultGameMaster.SetInfoAboutDiscoveredTaskField(new Location(2, 5), 0, 0, field, list);
            var responseField = list[0];

            Assert.AreEqual(2, responseField.x);
            Assert.AreEqual(5, responseField.y);
            Assert.IsTrue(responseField.playerIdSpecified);
            Assert.AreEqual(agent.ID, responseField.playerId);
            Assert.IsTrue(responseField.pieceIdSpecified);
            Assert.AreEqual(piece.id, responseField.pieceId);
        }

        [TestMethod]
        public void GameMasterSetsInfoAboutDiscoveredGoalkField()
        {
            List<Messages.GoalField> list = new List<Messages.GoalField>();
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0013");
            var field = defaultGameMaster.GetBoard.GetGoalField(2, 2);
            agent.SetLocation(2, 2);
            defaultGameMaster.RegisterAgent(agent, agent.GUID, findFreeLocationAndPlacePlayer: false);
            // set an agent on a TaskField
            var setPositionResult = defaultGameMaster.SetAbsoluteAgentLocation(2, 2, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(defaultGameMaster.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(true, setPositionResult);

            defaultGameMaster.SetInfoAboutDiscoveredGoalField(new Location(2, 2), 0, 0, field, list);
            var responseField = list[0];

            Assert.AreEqual(2, responseField.x);
            Assert.AreEqual(2, responseField.y);
            Assert.IsTrue(responseField.playerIdSpecified);
            Assert.AreEqual(agent.ID, responseField.playerId);
        }
    }
}
