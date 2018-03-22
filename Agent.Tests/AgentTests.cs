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
        public void PlayerWithGoalPieceTestsPiece()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition();
            var gameMaster = new GameArea.GameMaster(conf);
            var agent = new Player.Agent(TeamColour.blue, "testGUID-0002");
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

        
    }
}
