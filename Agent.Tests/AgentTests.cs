using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using System;
using Configuration;

namespace Player.Tests
{
    [TestClass]
    public class AgentTests
    {
        GameMasterSettings settings = GameMasterSettings.GetDefaultGameMasterSettings();
        GameMaster gameMaster = new GameArea.GameMaster(GameMasterSettings.GetDefaultGameMasterSettings());

        [TestMethod]
        public void NewAgent()
        {
            var agent = new Agent(TeamColour.blue);
            Assert.IsNotNull(agent);
            Assert.AreEqual(0u, agent.GetLocation.x);
            Assert.AreEqual(0u, agent.GetLocation.y);
            Assert.AreEqual(TeamColour.blue, agent.GetTeam);
            Assert.AreEqual("TEST_GUID", agent.GUID);
        }

        [TestMethod]
        public void GuidSet()
        {
            var agent = new Agent(TeamColour.blue);
            agent.SetGuid("kakao");
            Assert.AreEqual("kakao", agent.GUID);
        }

        [TestMethod]
        public void BoardSet()
        {
            var board = new Board(2u, 2u, 2u);
            var agent = new Agent(TeamColour.blue);
            Assert.IsNull(agent.GetBoard);
            agent.SetBoard(board);
            Assert.IsNotNull(agent.GetBoard);
            Assert.AreSame(board, agent.GetBoard);
        }

        [TestMethod]
        public void SetLocation()
        {
            var agent = new Agent(TeamColour.blue);
            var newLocation = new Location(2u, 3u);
            agent.SetLocation(newLocation);
            Assert.AreEqual(newLocation.x, agent.GetLocation.x);
            Assert.AreEqual(newLocation.y, agent.GetLocation.y);
        }

        // actions tests

        [TestMethod]
        public void PlayerWithoutPieceTestsPiece()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.red, "testGUID-0000");
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            var testResult = agent.TestPiece(gameMaster);

            Assert.AreEqual(false, testResult);
        }

        [TestMethod]
        public void PlayerWithShamPieceTestsPiece()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.red, "testGUID-0001");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 100)
            {
                timestamp = DateTime.Now,

            });


            var testResult = agent.TestPiece(gameMaster);

            Assert.AreEqual(true, testResult);
            Assert.AreEqual(PieceType.sham, agent.GetPiece.type);
        }

        [TestMethod]
        public void PlayerWithNormalPieceTestsPiece()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0002");
            // equip an agent with a normal piece
            agent.SetPiece(new Piece(PieceType.normal, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,

            });


            var testResult = agent.TestPiece(gameMaster);

            Assert.AreEqual(true, testResult);
            Assert.AreEqual(PieceType.normal, agent.GetPiece.type);
        }


        [TestMethod]
        public void PlayerPlacesShamPieceOnNotOccupiedTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0003");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 5); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0003"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNull(agent.GetPiece);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(90ul, (gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece.id);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnNotOccupiedTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0004");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 5); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0004"); // we change a location of GM's copy
            // assure there is no piece on the field
            gameMaster.GetBoard.GetTaskField(1, 5).SetPiece(null);

            // action: agent places a piece
            var actionResult = agent.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNull(agent.GetPiece);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(90ul, (gameMaster.GetBoard.GetField(1, 5) as GameArea.TaskField).GetPiece.id);
        }

        [TestMethod]
        public void PlayerPlacesShamPieceOnGoalField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0005");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 70)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 70)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 2); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 2, "testGUID-0005"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNotNull(agent.GetPiece);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnGoalFieldOfTypeGoal()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0006");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 23)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 23)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 1); // we change a location of an original object | (1,1) is GoalField of type 'goal' by default in GameMasterSettingsGameDefinition constructor

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 1, "testGUID-0006"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PlacePiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNull(agent.GetPiece);
        }

        [TestMethod]
        public void PlayerPlacesNormalPieceOnGoalFieldOfTypeNonGoal()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0007");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 23)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);
            agent.SetPiece(new Piece(PieceType.unknown, 23)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(4, 1);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(4, 1, "testGUID-0007"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PlacePiece(gameMaster);

            //Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNotNull(agent.GetPiece);
        }

        [TestMethod]
        public void PlayerPicksUpNormalPieceFromTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0008");
            agent.SetLocation(2, 5);

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.normal, 99);

            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 5, "testGUID-0008"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNotNull(agent.GetPiece);
            Assert.AreEqual(99ul, agent.GetPiece.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(PieceType.unknown, agent.GetPiece.type);
        }

        [TestMethod]
        public void PlayerPicksUpShamPieceFromTaskField()
        {
            var settings = GameMasterSettings.GetDefaultGameMasterSettings();
            var gameMaster = new GameArea.GameMaster(settings);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0009");
            agent.SetLocation(2, 5);

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.sham, 98);

            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 5, "testGUID-0009"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.IsNotNull(agent.GetPiece);
            Assert.AreEqual(98ul, agent.GetPiece.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(PieceType.unknown, agent.GetPiece.type);
        }

        [TestMethod]
        public void PlayerPicksUpFromEmptyTaskField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0010");
            agent.SetLocation(2, 5);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 5, "testGUID-0010"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
        }


        // these two cases are impossible to occurr
        //[TestMethod]
        //public void BluePlayerPicksUpFromRedGoalFieldOfTypeGoal()
        //{
        //    Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
        //    var gameMaster = new GameArea.GameMaster(conf);
        //    var agent = new Player.Agent(TeamColour.blue, "testGUID-0011");

        //    gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

        //    // set an agent on a TaskField
        //    var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 11, "testGUID-0011"); // we change a location of GM's copy

        //    // action: agent places a piece
        //    var actionResult = agent.PickUpPiece(gameMaster);

        //    Assert.AreEqual(false, setPositionResult);
        //    Assert.AreEqual(false, actionResult);
        //    Assert.IsNull(agent.GetPiece);
        //}

        //[TestMethod]
        //public void BluePlayerPicksUpFromRedGoalFieldOfTypeNonGoal()
        //{
        //    Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
        //    var gameMaster = new GameArea.GameMaster(conf);
        //    var agent = new Player.Agent(TeamColour.blue, "testGUID-0012");

        //    gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

        //    // set an agent on a TaskField
        //    var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 12, "testGUID-0012"); // we change a location of GM's copy

        //    // action: agent places a piece
        //    var actionResult = agent.PickUpPiece(gameMaster);

        //    Assert.AreEqual(false, setPositionResult);
        //    Assert.AreEqual(false, actionResult);
        //    Assert.IsNull(agent.GetPiece);
        //}

        [TestMethod]
        public void BluePlayerPicksUpFromBlueGoalFieldOfTypeGoal()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0011");
            agent.SetLocation(2, 2);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 2, "testGUID-0011"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
        }

        [TestMethod]
        public void BluePlayerPicksUpFromBlueGoalFieldOfTypeNonGoal()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0012");
            agent.SetLocation(2, 1);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 1, "testGUID-0012"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
        }

        /// MOVE tests

        [TestMethod]
        public void PlayerMovesFromTaskFieldToEmptyTaskField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0013");
            agent.SetLocation(2, 4);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 4, "testGUID-0013"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 4).Player);

            // action: agent moves up to (2,5)
            var actionResult = agent.Move(gameMaster, MoveType.up);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            var temp = gameMaster.GetBoard.GetField(2, 4).GetFieldType;
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 4).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 5).GetFieldType);
            Assert.AreEqual(new Location(2, 5), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(2, 5).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToOccupiedTaskField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0014");
            var strangerAgent = new Player.Agent(TeamColour.blue, "testGUID-0015");
            agent.SetLocation(2, 4);
            strangerAgent.SetLocation(2, 5);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterAgent(strangerAgent,strangerAgent.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set agents on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 4, "testGUID-0014");
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(2, 5, "testGUID-0015"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 5).Player);

            // action: agent moves up to (2,5)
            var actionResult = agent.Move(gameMaster, MoveType.up);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 4).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 5).GetFieldType);
            Assert.AreEqual(new Location(2, 4), agent.GetLocation);
            Assert.AreEqual(new Location(2, 5), strangerAgent.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 4).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(2, 4).Player.id);
            Assert.AreEqual(strangerAgent.ID, gameMaster.GetBoard.GetField(2, 5).Player.id);
            Assert.AreEqual(strangerAgent.ID, agent.GetBoard.GetField(2, 5).Player.id); // check if agent has stored an encountered stranger in his board view

        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToRightEmptyGoalField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0016");
            agent.SetLocation(2, 3);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 3, "testGUID-0016"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 3).Player);

            // action: agent moves up to (2,2)
            var actionResult = agent.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 2).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 3).GetFieldType);
            Assert.AreEqual(new Location(2, 2), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(2, 2).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToToRightOccupiedGoalField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0017");
            var strangerAgent = new Player.Agent(TeamColour.blue, "testGUID-0018");
            agent.SetLocation(2, 3);
            strangerAgent.SetLocation(2, 2);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterAgent(strangerAgent,strangerAgent.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set agents on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 3, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(2, 2, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 2).Player);

            // action: agent down up to (2,2)
            var actionResult = agent.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 2).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(2, 3).GetFieldType);
            Assert.AreEqual(new Location(2, 3), agent.GetLocation);
            Assert.AreEqual(new Location(2, 2), strangerAgent.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 2).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(2, 3).Player.id);
            Assert.AreEqual(strangerAgent.ID, gameMaster.GetBoard.GetField(2, 2).Player.id);
            Assert.AreEqual(strangerAgent.ID, agent.GetBoard.GetField(2, 2).Player.id); // check if agent has stored an encountered stranger in his board view
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldToWrongGoalField()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0019");
            agent.SetLocation(1, 9);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 9, "testGUID-0019"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 9).Player);

            // action: agent moves up to (1,10)
            var actionResult = agent.Move(gameMaster, MoveType.up);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(1, 9).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(1, 10).GetFieldType);
            Assert.AreEqual(new Location(1, 9), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 9).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(1, 9).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromTaskFieldOutOfBoard()
        {
            var agent = new Player.Agent(TeamColour.red, "testGUID-0017");
            var agent2 = new Player.Agent(TeamColour.blue, "testGUID-0018");
            agent.SetLocation(4, 7);
            agent2.SetLocation(0, 5);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterAgent(agent2,agent2.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set agents on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(4, 7, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(0, 5, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 7).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 5).Player);

            var actionResult = agent.Move(gameMaster, MoveType.right);
            var actionResult2 = agent2.Move(gameMaster, MoveType.left);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(false, actionResult2);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(4, 7).GetFieldType);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(0, 5).GetFieldType);
            Assert.AreEqual(new Location(4, 7), agent.GetLocation);
            Assert.AreEqual(new Location(0, 5), agent2.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 7).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 5).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(4, 7).Player.id);
            Assert.AreEqual(agent2.ID, gameMaster.GetBoard.GetField(0, 5).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToEmptyTaskField()
        {

            var agent = new Player.Agent(TeamColour.red, "testGUID-0020");
            agent.SetLocation(1, 10);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 10, "testGUID-0020"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 10).Player);

            // action: agent moves up to (1,9)
            var actionResult = agent.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(1, 9).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(1, 10).GetFieldType);
            Assert.AreEqual(new Location(1, 9), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 9).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(1, 9).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToOccupiedTaskField()
        {
            var agent = new Player.Agent(TeamColour.red, "testGUID-0014");
            var strangerAgent = new Player.Agent(TeamColour.blue, "testGUID-0015");
            agent.SetLocation(3, 10);
            strangerAgent.SetLocation(3, 9);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterAgent(strangerAgent,strangerAgent.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set agents on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(3, 10, "testGUID-0014");
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(3, 9, "testGUID-0015"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 9).Player);

            // action: agent moves up to (3,9)
            var actionResult = agent.Move(gameMaster, MoveType.down);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Task, gameMaster.GetBoard.GetField(3, 9).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(3, 10).GetFieldType);
            Assert.AreEqual(new Location(3, 10), agent.GetLocation);
            Assert.AreEqual(new Location(3, 9), strangerAgent.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 10).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 9).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(3, 10).Player.id);
            Assert.AreEqual(strangerAgent.ID, gameMaster.GetBoard.GetField(3, 9).Player.id);
            Assert.AreEqual(strangerAgent.ID, agent.GetBoard.GetField(3, 9).Player.id); // check if agent has stored an encountered stranger in his board view

        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToEmptyGoalField()
        {
            var agent = new Player.Agent(TeamColour.red, "testGUID-0016");
            agent.SetLocation(1, 12);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 12, "testGUID-0016"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 12).Player);

            // action: agent moves up to (2,12)
            var actionResult = agent.Move(gameMaster, MoveType.right);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(1, 12).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 12).GetFieldType);
            Assert.AreEqual(new Location(2, 12), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(1, 12).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 12).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(2, 12).Player.id);
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldToToOccupiedGoalField()
        {
            var agent = new Player.Agent(TeamColour.red, "testGUID-0017");
            var strangerAgent = new Player.Agent(TeamColour.red, "testGUID-0018");
            agent.SetLocation(3, 11);
            strangerAgent.SetLocation(2, 11);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterAgent(strangerAgent,strangerAgent.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set agents on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(3, 11, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(2, 11, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 11).Player);

            // action: agent down up to (2,11)
            var actionResult = agent.Move(gameMaster, MoveType.left);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(2, 11).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(3, 11).GetFieldType);
            Assert.AreEqual(new Location(3, 11), agent.GetLocation);
            Assert.AreEqual(new Location(2, 11), strangerAgent.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(3, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(2, 11).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(3, 11).Player.id);
            Assert.AreEqual(strangerAgent.ID, gameMaster.GetBoard.GetField(2, 11).Player.id);
            Assert.AreEqual(strangerAgent.ID, agent.GetBoard.GetField(2, 11).Player.id); // check if agent has stored an encountered stranger in his board view
        }

        [TestMethod]
        public void PlayerMovesFromGoalFieldOutOfBoard()
        {
            var agent = new Player.Agent(TeamColour.red, "testGUID-0017");
            var agent2 = new Player.Agent(TeamColour.blue, "testGUID-0018");
            agent.SetLocation(4, 11);
            agent2.SetLocation(0, 1);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false); // id = 1
            gameMaster.RegisterAgent(agent2,agent2.GUID, findFreeLocationAndPlacePlayer : false); // id = 2

            // set agents on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(4, 11, "testGUID-0017");
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(0, 1, "testGUID-0018"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 1).Player);

            var actionResult = agent.Move(gameMaster, MoveType.right);
            var actionResult2 = agent2.Move(gameMaster, MoveType.left);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(false, actionResult);
            Assert.AreEqual(false, actionResult2);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(4, 11).GetFieldType);
            Assert.AreEqual(FieldType.Goal, gameMaster.GetBoard.GetField(0, 1).GetFieldType);
            Assert.AreEqual(new Location(4, 11), agent.GetLocation);
            Assert.AreEqual(new Location(0, 1), agent2.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 11).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 1).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(4, 11).Player.id);
            Assert.AreEqual(agent2.ID, gameMaster.GetBoard.GetField(0, 1).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoveryNothingInSight()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0020");
            agent.SetLocation(1, 5);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0020"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);


            // action: agent discovers area
            agent.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(new Location(1, 5), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.IsNull(agent.GetBoard.GetField(2, 5).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(1, 5).Player.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            Assert.IsNull((agent.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
            //Assert.AreEqual(1, (agent.GetBoard.GetField(2, 5) as GameArea.TaskField).Distance);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPiece()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0021");
            agent.SetLocation(1, 6);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 6, "testGUID-0021"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 6, TeamColour.blue, PieceType.sham, 98);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece);


            // action: agent discovers area
            agent.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(new Location(1, 6), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(1, 6).Player.id);
            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece);
            Assert.IsNotNull((agent.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece);
            Assert.AreEqual(98ul, (gameMaster.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece.id);
            Assert.AreEqual(98ul, (agent.GetBoard.GetField(2, 6) as GameArea.TaskField).GetPiece.id);
            Assert.IsNull((gameMaster.GetBoard.GetField(1, 6) as GameArea.TaskField).GetPiece);
            //Assert.AreEqual(1, (agent.GetBoard.GetField(1, 5) as GameArea.TaskField).Distance);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPlayerInTaskArea()
        {
            var agent1 = new Player.Agent(TeamColour.blue, "testGUID-0022");
            var agent2 = new Player.Agent(TeamColour.blue, "testGUID-0023");
            agent1.SetLocation(1, 6);
            agent2.SetLocation(1, 5);

            gameMaster.RegisterAgent(agent1,agent1.GUID, findFreeLocationAndPlacePlayer : false);
            gameMaster.RegisterAgent(agent2,agent2.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult1 = gameMaster.SetAbsoluteAgentLocation(1, 6, "testGUID-0022"); // we change a location of GM's copy
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0023"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);


            // action: agent discovers area
            agent1.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult1);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(new Location(1, 6), agent1.GetLocation);
            Assert.AreEqual(new Location(1, 5), agent2.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 6).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 5).Player);
            Assert.IsNotNull(agent1.GetBoard.GetField(1, 5).Player);
            Assert.AreEqual(agent2.ID, agent1.GetBoard.GetField(1, 5).Player.id);
            Assert.AreEqual(gameMaster.GetBoard.GetField(1, 5).Player.id, agent1.GetBoard.GetField(1, 5).Player.id);
            Assert.AreEqual(agent1.ID, gameMaster.GetBoard.GetField(1, 6).Player.id);
            Assert.AreEqual(agent2.ID, gameMaster.GetBoard.GetField(1, 5).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoverySeesPlayerInGoalArea()
        {
            var agent1 = new Player.Agent(TeamColour.blue, "testGUID-0024");
            var agent2 = new Player.Agent(TeamColour.blue, "testGUID-0025");
            agent1.SetLocation(1, 0);
            agent2.SetLocation(1, 1);

            gameMaster.RegisterAgent(agent1,agent1.GUID, findFreeLocationAndPlacePlayer : false);
            gameMaster.RegisterAgent(agent2,agent2.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult1 = gameMaster.SetAbsoluteAgentLocation(1, 0, "testGUID-0024"); // we change a location of GM's copy
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(1, 1, "testGUID-0025"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 0).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 1).Player);


            // action: agent discovers area
            agent1.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult1);
            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(new Location(1, 0), agent1.GetLocation);
            Assert.AreEqual(new Location(1, 1), agent2.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 3).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 0).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(1, 1).Player);
            Assert.IsNotNull(agent1.GetBoard.GetField(1, 1).Player);
            Assert.AreEqual(agent2.ID, agent1.GetBoard.GetField(1, 1).Player.id);
            Assert.AreEqual(gameMaster.GetBoard.GetField(1, 1).Player.id, agent1.GetBoard.GetField(1, 1).Player.id);
            Assert.AreEqual(agent1.ID, gameMaster.GetBoard.GetField(1, 0).Player.id);
            Assert.AreEqual(agent2.ID, gameMaster.GetBoard.GetField(1, 1).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoveryNearBoardEdge()
        {
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0026");
            agent.SetLocation(0, 3);

            gameMaster.RegisterAgent(agent,agent.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(0, 3, "testGUID-0026"); // we change a location of GM's copy

            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 3).Player);


            // action: agent discovers area
            agent.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(new Location(0, 3), agent.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.IsNull(agent.GetBoard.GetField(2, 5).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 3).Player);
            Assert.AreEqual(agent.ID, gameMaster.GetBoard.GetField(0, 3).Player.id);
        }

        [TestMethod]
        public void PlayerDiscoveryCorner()
        {
            var agent1 = new Player.Agent(TeamColour.blue, "testGUID-0027");
            var agent2 = new Player.Agent(TeamColour.red, "testGUID-0028");
            agent1.SetLocation(0, 0);
            agent2.SetLocation(4, 9);

            gameMaster.RegisterAgent(agent1,agent1.GUID, findFreeLocationAndPlacePlayer : false);
            gameMaster.RegisterAgent(agent2,agent2.GUID, findFreeLocationAndPlacePlayer : false);

            // set an agent on a TaskField
            var setPositionResult1 = gameMaster.SetAbsoluteAgentLocation(0, 0, "testGUID-0027"); // we change a location of GM's copy
            var setPositionResult2 = gameMaster.SetAbsoluteAgentLocation(4, 9, "testGUID-0028"); // we change a location of GM's copy
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 0).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 9).Player);


            // action: agent discovers area
            agent1.Discover(gameMaster);

            Assert.AreEqual(true, setPositionResult1);
            Assert.AreEqual(new Location(0, 0), agent1.GetLocation);
            Assert.IsNull(gameMaster.GetBoard.GetField(2, 5).Player);
            Assert.IsNull(agent1.GetBoard.GetField(2, 5).Player);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(0, 0).Player);
            Assert.AreEqual(agent1.ID, gameMaster.GetBoard.GetField(0, 0).Player.id);

            Assert.AreEqual(true, setPositionResult2);
            Assert.AreEqual(new Location(4, 9), agent2.GetLocation);
            Assert.IsNotNull(gameMaster.GetBoard.GetField(4, 9).Player);
            Assert.AreEqual(agent2.ID, gameMaster.GetBoard.GetField(4, 9).Player.id);
        }
    }
}
