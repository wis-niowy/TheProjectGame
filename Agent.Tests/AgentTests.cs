using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using System;

namespace Player.Tests
{
    [TestClass]
    public class AgentTests
    {

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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition()
            {
                //BoardWidth = "5",
                //GoalAreaLength = "2",
                //TaskAreaLength = "4",
                //ShamProbability = 0.5,
                //InitialNumberOfPieces = 5,
                //NumberOfPlayersPerTeam = "4",
                //GameName = "Test Game",
                //PlacingNewPiecesFrequency = 4

            };
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.red, "testGUID-0000");
            gameMaster.RegisterAgent(agent);

            var testResult = agent.TestPiece(gameMaster);

            Assert.AreEqual(false, testResult);
        }

        [TestMethod]
        public void PlayerWithShamPieceTestsPiece()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.red, "testGUID-0001");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 100)
            {
                timestamp = DateTime.Now,
                
            });
            gameMaster.RegisterAgent(agent);
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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0002");
            // equip an agent with a normal piece
            agent.SetPiece(new Piece(PieceType.normal, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent);
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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0003");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent);
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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0004");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 90)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent);
            agent.SetPiece(new Piece(PieceType.unknown, 90)
            {
                timestamp = DateTime.Now,
            });
            agent.SetLocation(1, 5); // we change a location of an original object

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(1, 5, "testGUID-0004"); // we change a location of GM's copy

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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0005");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.sham, 70)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent);
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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0006");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 23)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent);
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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0007");
            // equip an agent with a sham piece
            agent.SetPiece(new Piece(PieceType.normal, 23)
            {
                timestamp = DateTime.Now,

            });
            gameMaster.RegisterAgent(agent);
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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0008");

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.normal, 99);

            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);

            gameMaster.RegisterAgent(agent);

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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0009");

            // place a piece on a TaskField
            gameMaster.SetPieceInLocation(2, 5, TeamColour.blue, PieceType.sham, 98);

            Assert.IsNotNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);

            gameMaster.RegisterAgent(agent);

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
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0010");

            gameMaster.RegisterAgent(agent);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 5, "testGUID-0010"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(true, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
            Assert.IsNull((gameMaster.GetBoard.GetField(2, 5) as GameArea.TaskField).GetPiece);
        }

        [TestMethod]
        public void BluePlayerPicksUpFromRedGoalFieldOfTypeGoal()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0011");

            gameMaster.RegisterAgent(agent);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 11, "testGUID-0011"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(false, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
        }

        [TestMethod]
        public void BluePlayerPicksUpFromRedGoalFieldOfTypeNonGoal()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0012");

            gameMaster.RegisterAgent(agent);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 12, "testGUID-0012"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(false, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
        }

        [TestMethod]
        public void BluePlayerPicksUpFromBlueGoalFieldOfTypeGoal()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0011");

            gameMaster.RegisterAgent(agent);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 2, "testGUID-0011"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(false, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
        }

        [TestMethod]
        public void BluePlayerPicksUpFromBlueGoalFieldOfTypeNonGoal()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0012");

            gameMaster.RegisterAgent(agent);

            // set an agent on a TaskField
            var setPositionResult = gameMaster.SetAbsoluteAgentLocation(2, 1, "testGUID-0012"); // we change a location of GM's copy

            // action: agent places a piece
            var actionResult = agent.PickUpPiece(gameMaster);

            Assert.AreEqual(false, setPositionResult);
            Assert.AreEqual(false, actionResult);
            Assert.IsNull(agent.GetPiece);
        }
    }
}
